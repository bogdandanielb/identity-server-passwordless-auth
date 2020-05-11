using System;
using System.Threading.Tasks;
using DanielBogdan.Passwordless.Identity.Core;
using DanielBogdan.Passwordless.Identity.Core.Configuration;
using DanielBogdan.Passwordless.Identity.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DanielBogdan.Passwordless.Identity.Controllers
{
    [Authorize]
    [Route("change-password")]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly ILogger _logger;
        private readonly SiteConfiguration _siteConfiguration;

        public ManageController(UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager,
                IIdentityServerInteractionService interaction,
                ILogger<ManageController> logger,
                IOptionsMonitor<SiteConfiguration> siteConfiguration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _logger = logger;
            _siteConfiguration = siteConfiguration?.CurrentValue ?? throw new ArgumentNullException(nameof(siteConfiguration));
        }

        [HttpGet("", Name = "change-password")]
        public IActionResult ChangePassword(string redirectUri)
        {
            var model = new ChangePasswordViewModel()
            {
                ReturnUrl = redirectUri
            };

            return View(model);
        }

        [HttpPost("", Name = "change-password-submit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("Unable to load user with ID '{UserId}'.", _userManager.GetUserId(User));
                throw new PasswordlessException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User {UserId} changed their password successfully.", _userManager.GetUserId(User));

            return View("ChangePasswordSuccess", new ChangePasswordResultViewModel()
            {
                IsSuccess = true,
                ReturnUrl = model.ReturnUrl
            });
        }
    }
}
