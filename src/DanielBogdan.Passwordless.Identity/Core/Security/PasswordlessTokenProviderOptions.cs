using Microsoft.AspNetCore.Identity;
using System;

namespace DanielBogdan.Passwordless.Identity.Core.Security
{
    public class PasswordlessTokenProviderOptions : DataProtectionTokenProviderOptions
    {
        public PasswordlessTokenProviderOptions()
        {
            // update the defaults
            Name = "PasswordlessTokenProvider";
            TokenLifespan = TimeSpan.FromMinutes(15);
        }
    }
}
