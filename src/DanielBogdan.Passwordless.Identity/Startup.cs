using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using DanielBogdan.Passwordless.Identity.Abstractions;
using DanielBogdan.Passwordless.Identity.Core.Configuration;
using DanielBogdan.Passwordless.Identity.Core.Security;
using DanielBogdan.Passwordless.Identity.Data;
using DanielBogdan.Passwordless.Identity.Email;
using DanielBogdan.Passwordless.Identity.Helpers;
using DanielBogdan.Passwordless.Identity.Models;
using DanielBogdan.Passwordless.Identity.Services;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SendGrid;

namespace DanielBogdan.Passwordless.Identity
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<DataConfiguration>(Configuration.GetSection(nameof(DataConfiguration)));
            services.Configure<SiteConfiguration>(Configuration.GetSection(nameof(SiteConfiguration)));
            services.Configure<EmailConfiguration>(Configuration.GetSection(nameof(EmailConfiguration)));

            var dataConfiguration = Configuration.GetSection(nameof(DataConfiguration)).Get<DataConfiguration>();
            var emailConfiguration = Configuration.GetSection(nameof(EmailConfiguration)).Get<EmailConfiguration>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(dataConfiguration.ConnectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>(o =>
            {
                // Password settings.
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireUppercase = false;
                o.Password.RequiredLength = 6;
                o.Password.RequiredUniqueChars = 1;

                // User settings.
                o.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddPasswordlessTokenProvider();

            services.AddControllersWithViews()
                    .AddRazorRuntimeCompilation()
                    .AddNewtonsoftJson();

            services.AddMemoryCache();
            services.AddHttpContextAccessor();

            services.AddCors(o =>
            {
                o.AddDefaultPolicy(b => b.AllowAnyHeader()
                                       .AllowAnyMethod()
                                       .WithOrigins(Configuration.GetSection("IdentityServer:Clients").Get<Client[]>().SelectMany(p => p.AllowedCorsOrigins).ToArray())
                                       .AllowCredentials());
            });

            services.AddAuthentication();

            var certificateConfiguration = Configuration.GetSection("IdentityServer:Certificate").Get<CertificateConfiguration>();
            var certificate = new X509Certificate2(Convert.FromBase64String(certificateConfiguration.Content), certificateConfiguration.Password);

            var builder = services.AddIdentityServer(o =>
            {
                o.UserInteraction.LoginUrl = "/login";
                o.UserInteraction.LogoutUrl = "/logout";
                o.UserInteraction.ConsentUrl = "/consent";
                o.UserInteraction.ErrorUrl = "/error";

                o.Events.RaiseErrorEvents = true;
                o.Events.RaiseInformationEvents = true;
                o.Events.RaiseFailureEvents = true;
                o.Events.RaiseSuccessEvents = true;
            })
            .AddInMemoryIdentityResources(Configuration.GetSection("IdentityServer:IdentityResources"))
            .AddInMemoryApiResources(Configuration.GetSection("IdentityServer:ApiResources"))
            .AddInMemoryClients(Configuration.GetSection("IdentityServer:Clients"))
            .AddAspNetIdentity<ApplicationUser>()
            .AddProfileService<ProfileService>()
            .AddSigningCredential(certificate);

            services.AddTransient<ISendGridClient, SendGridClient>(sp => new SendGridClient(emailConfiguration.ApiKey));
            services.AddTransient<IProfileService, ProfileService>();
            services.AddTransient<IEmailMessageBuilder, EmailMessageBuilder>();

            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IPasswordlessService, PasswordlessService>();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Add(new IPNetwork(
                        System.Net.IPAddress.Parse("::ffff:10.0.0.0"), 104));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/error/{0}");
                app.UseExceptionHandler("/error/500");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors();

            app.UseIdentityServer();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

            app.EnsureDatabase();
            app.SeedUsers();
        }

    }
}
