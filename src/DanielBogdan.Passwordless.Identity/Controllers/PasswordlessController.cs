using System;
using System.Threading.Tasks;
using System.Web;
using DanielBogdan.Passwordless.Identity.Abstractions;
using DanielBogdan.Passwordless.Identity.Core;
using DanielBogdan.Passwordless.Identity.Core.Configuration;
using DanielBogdan.Passwordless.Identity.Models;
using IdentityServer4.Events;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DanielBogdan.Passwordless.Identity.Controllers
{
    [AllowAnonymous]
    [Route("passwordless")]
    public class PasswordlessController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEventService _events;
        private readonly ILogger _logger;
        private readonly IPasswordlessService _passwordlessService;
        private readonly SiteConfiguration _siteConfiguration;

        public PasswordlessController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IEventService events,
            ILogger<PasswordlessController> logger,
            IPasswordlessService passwordlessService,
            IOptionsMonitor<SiteConfiguration> siteConfiguration)
        {
            _interaction = interaction;
            _signInManager = signInManager;
            _events = events;
            _logger = logger;
            _passwordlessService = passwordlessService;
            _siteConfiguration = siteConfiguration?.CurrentValue ?? throw new ArgumentNullException(nameof(siteConfiguration));
        }

        [HttpGet("", Name = "passwordless")]
        public IActionResult Index(string returnUrl)
        {
            return View(new PasswordlessInputModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost("", Name = "passwordless-submit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(PasswordlessInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            try
            {
                var accountsUrl = Url.RouteUrl("passwordless-login", null, Request.Scheme);
                await _passwordlessService.SendAsync(context?.ClientId ?? "angular-client", model.Email, accountsUrl);
            }
            catch (PasswordlessException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);

                return View(model);
            }

            return View("Success", model);
        }

        [HttpGet("login", Name = "passwordless-login")]
        public async Task<IActionResult> Login(string clientId, string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return View("Error", new PasswordlessErrorModel() { Message = $"Token not found or expired." });
            }

            // Validate token
            var validationResult = await _passwordlessService.ValidateAsync(userId, token);
            if (validationResult == null)
            {
                return View("Error", new PasswordlessErrorModel() { Message = $"Token not found or expired." });
            }

            await _events.RaiseAsync(new UserLoginSuccessEvent(validationResult.User.UserName, validationResult.User.Id, validationResult.User.UserName, clientId: clientId));

            await _signInManager.SignInAsync(validationResult.User, true, "passwordless");

            string baseUrl;
            switch (clientId)
            {
                case "angular-client":
                    {
                        baseUrl = _siteConfiguration.AngularClientUrl;
                        break;
                    }
                case "react-client":
                    {
                        baseUrl = _siteConfiguration.ReactClientUrl;
                        break;
                    }
                case "vue-client":
                    {
                        baseUrl = _siteConfiguration.VueClientUrl;
                        break;
                    }
                default:
                    {
                        baseUrl = _siteConfiguration.IdentityUrl;
                        break;
                    }
            }

            return Redirect($"{baseUrl}/passwordless?userId={HttpUtility.UrlEncode(userId)}&token={HttpUtility.UrlEncode(token)}");
        }
    }
}
