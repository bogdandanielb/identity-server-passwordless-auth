using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityModel;

namespace DanielBogdan.Passwordless.Identity.Helpers
{
    public static class IdentityExtensions
    {
        public static string GetUserName(this ClaimsPrincipal identity)
        {
            return identity.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static string GetEmail(this ClaimsPrincipal identity)
        {
            return identity.FindFirstValue(JwtClaimTypes.Email);
        }

        public static string GetPhone(this ClaimsPrincipal identity)
        {
            return identity.FindFirstValue(JwtClaimTypes.PhoneNumber);
        }

        public static string GetDisplayName(this ClaimsPrincipal identity)
        {
            string givenName = identity.FindFirstValue(JwtClaimTypes.GivenName);
            string surName = identity.FindFirstValue(JwtClaimTypes.FamilyName);

            return string.Format("{0} {1}", givenName.ToTitleCase(), surName.ToTitleCase());
        }

        public static string GetName(this ClaimsPrincipal identity)
        {
            string name = identity.FindFirstValue(JwtClaimTypes.Name);

            return string.Format("{0}", name);
        }

        public static string GetFirstName(this ClaimsPrincipal identity)
        {
            string givenName = identity.FindFirstValue(JwtClaimTypes.GivenName);

            return string.Format("{0}", givenName.ToTitleCase());
        }

        public static string GetLastName(this ClaimsPrincipal identity)
        {
            string surName = identity.FindFirstValue(JwtClaimTypes.FamilyName);

            return string.Format("{0}", surName.ToTitleCase());
        }

        public static IEnumerable<string> GetUserRoles(this ClaimsPrincipal identity)
        {
            return identity.Claims
                            .Where(c => c.Type == ClaimTypes.Role || c.Type == JwtClaimTypes.Role)
                            .Select(c => c.Value)
                            .ToList();
        }
    }
}
