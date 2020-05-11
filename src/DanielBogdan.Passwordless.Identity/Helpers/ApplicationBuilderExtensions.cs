using System.Linq;
using DanielBogdan.Passwordless.Identity.Core;
using DanielBogdan.Passwordless.Identity.Data;
using DanielBogdan.Passwordless.Identity.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DanielBogdan.Passwordless.Identity.Helpers
{
    public static class ApplicationBuilderExtensions
    {
        public static void EnsureDatabase(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                context.Database.Migrate();
            }
        }

        public static void SeedUsers(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<Startup>>();

                if (userManager.FindByNameAsync("bogdandanielb@gmail.com").Result == null)
                {
                    var user = new ApplicationUser
                    {
                        FirstName = "Daniel",
                        LastName = "Bogdan",
                        Email = "bogdandanielb@gmail.com",
                        PhoneNumber = "1234567890",
                        UserName = "bogdandanielb@gmail.com"
                    };

                    var result = userManager.CreateAsync(user, "Pass123$").Result;
                    if (!result.Succeeded)
                    {
                        throw new PasswordlessException(result.Errors.First().Description);
                    }

                    logger.LogInformation("User has been created");
                }
                else
                {
                    logger.LogDebug("User already exists");
                }
            }
        }
    }
}
