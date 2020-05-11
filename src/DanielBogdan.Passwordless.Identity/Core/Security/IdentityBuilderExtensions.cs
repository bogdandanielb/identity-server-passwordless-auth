using Microsoft.AspNetCore.Identity;

namespace DanielBogdan.Passwordless.Identity.Core.Security
{
    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder AddPasswordlessTokenProvider(this IdentityBuilder builder)
        {
            var userType = builder.UserType;
            var provider = typeof(PasswordlessTokenProvider<>).MakeGenericType(userType);
            return builder.AddTokenProvider("PasswordlessProvider", provider);
        }
    }
}
