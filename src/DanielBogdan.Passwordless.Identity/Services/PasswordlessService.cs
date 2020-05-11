using System;
using System.IO;
using System.Threading.Tasks;
using DanielBogdan.Passwordless.Identity.Abstractions;
using DanielBogdan.Passwordless.Identity.Core;
using DanielBogdan.Passwordless.Identity.Core.Configuration;
using DanielBogdan.Passwordless.Identity.Email;
using DanielBogdan.Passwordless.Identity.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DanielBogdan.Passwordless.Identity.Services
{
    public class PasswordlessService : IPasswordlessService
    {
        private const string TokenProvider = "PasswordlessProvider";
        private const string Purpose = "PasswordlessAuth";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailMessageBuilder _emailMessageBuilder;
        private readonly IEmailService _emailService;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _environment;

        public PasswordlessService(
            UserManager<ApplicationUser> userManager,
            IEmailMessageBuilder emailMessageBuilder,
            IEmailService emailService,
               IOptionsMonitor<EmailConfiguration> emailConfiguration,
            ILogger<PasswordlessService> logger,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _emailMessageBuilder = emailMessageBuilder;
            _emailService = emailService;
            _logger = logger;
            _environment = environment;
            _emailConfiguration = emailConfiguration.CurrentValue ?? throw new ArgumentNullException(nameof(emailConfiguration));
        }

        public async Task SendAsync(string clientId, string email, string accountsUrl)
        {
            // verify email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new PasswordlessException("User not found.");
            }

            var token = await _userManager.GenerateUserTokenAsync(user, TokenProvider, Purpose);

            _logger.LogDebug($"Passwordless token: {token}");

            var url = $"{accountsUrl}?clientId={System.Web.HttpUtility.UrlEncode(clientId)}&userId={System.Web.HttpUtility.UrlEncode(user.Id)}&token={System.Web.HttpUtility.UrlEncode(token)}";

            _logger.LogDebug($"Dispatching Passwordless: {url}");

            var body = _emailMessageBuilder.Init(File.ReadAllText(Path.Combine(_environment.ContentRootPath, TemplateConstants.TemplatesPath, TemplateConstants.Passwordless)))
                .AddSubstitution(SubstitutionTypes.UserFirstName, user.FirstName)
                .AddSubstitution(SubstitutionTypes.UserLastName, user.LastName)
                .AddSubstitution(SubstitutionTypes.UserEmail, user.Email)
                .AddSubstitution(SubstitutionTypes.ActionLink, url)
                .Build();
            await _emailService.SendAsync(new HtmlEmailMessage
            {
                From = _emailConfiguration.SupportEmail,
                To = user.Email,
                CC = _emailConfiguration.CC,
                BCC = _emailConfiguration.BCC,
                Subject = "Authentication link",
                Body = body
            });
        }

        public async Task<PasswordlessValidationResult> ValidateAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            var isValid = await _userManager.VerifyUserTokenAsync(user, TokenProvider, Purpose, token);
            if (isValid == false)
            {
                return null;
            }

            return new PasswordlessValidationResult()
            {
                User = user,
                Token = token
            };
        }
    }
}
