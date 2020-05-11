using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DanielBogdan.Passwordless.Identity.Models;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DanielBogdan.Passwordless.Identity.Core.Security
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;
        private readonly ILogger<ProfileService> _logger;

        public ProfileService(UserManager<ApplicationUser> userManager,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
            ILogger<ProfileService> logger)
        {
            _userManager = userManager;
            _claimsFactory = claimsFactory;
            _logger = logger;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject?.GetSubjectId();
            if (sub == null) { throw new PasswordlessException("No sub claim present"); }

            var user = await _userManager.FindByIdAsync(sub);
            var principal = await _claimsFactory.CreateAsync(user);
            if (principal == null) { throw new PasswordlessException("Failed to create a principal"); }

            var replaceClaims = new[] { JwtClaimTypes.Name };
            var claims = principal.Claims.Where(c => !replaceClaims.Contains(c.Type)).ToList();
            //TODO: Consider filtering retrned claims by requested claims
            //claims = context.FilterClaims(claims).Where(c => !replaceClaims.Contains(c.Type)).ToList();

            claims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName));
            claims.Add(new Claim(JwtClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim()));
            if (string.IsNullOrWhiteSpace(user.FirstName) == false) { claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName)); }
            if (string.IsNullOrWhiteSpace(user.PhoneNumber) == false) { claims.Add(new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber)); }

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject?.GetSubjectId();
            if (sub == null) throw new PasswordlessException("No subject Id claim present");

            var user = await _userManager.FindByIdAsync(sub);
            if (user == null)
            {
                _logger.LogWarning("No user found matching subject Id: {0}", sub);
            }

            context.IsActive = user != null;
        }
    }
}
