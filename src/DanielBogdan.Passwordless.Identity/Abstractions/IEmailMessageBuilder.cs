using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DanielBogdan.Passwordless.Identity.Email;

namespace DanielBogdan.Passwordless.Identity.Abstractions
{
    public interface IEmailMessageBuilder
    {
        IEmailMessageBuilder Init(string template);
        IEmailMessageBuilder AddSubstitution(string key, string value);
        IEmailMessageBuilder AddSubstitution(SubstitutionTypes substitutionType, string value);
        string Build();
    }
}
