using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DanielBogdan.Passwordless.Identity.Abstractions;
using DanielBogdan.Passwordless.Identity.Helpers;

namespace DanielBogdan.Passwordless.Identity.Email
{
    public class EmailMessageBuilder : IEmailMessageBuilder
    {
        private static readonly string LeftSeparator = "{{";
        private static readonly string RightSeparator = "}}";
        private static readonly IEnumerable<string> AllSubstitutionTypes = Enum.GetNames(typeof(SubstitutionTypes));

        private IDictionary<string, string> _substitutions;
        private string _template;

        public EmailMessageBuilder()
        {
        }

        public IEmailMessageBuilder Init(string template)
        {
            _substitutions = new Dictionary<string, string>();
            _template = template;

            return this;
        }

        public virtual string Build()
        {
            foreach (string substitutionType in AllSubstitutionTypes)
            {
                _template = Regex.Replace(_template, LeftSeparator + substitutionType + RightSeparator,
                    _substitutions.TryGetValue(substitutionType, out string substitutionValue) && substitutionValue != null ? substitutionValue : string.Empty,
                    RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }

            return _template;
        }

        public virtual IEmailMessageBuilder AddSubstitution(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            _substitutions[key] = value;
            return this;
        }

        public virtual IEmailMessageBuilder AddSubstitution(SubstitutionTypes substitutionType, string value)
        {
            return AddSubstitution(substitutionType.ToString(), value);
        }

        public virtual IEmailMessageBuilder AddSubstitutions(IDictionary<string, string> substitutions)
        {
            if (substitutions == null) throw new ArgumentNullException(nameof(substitutions));

            _substitutions.AddRange(substitutions);
            return this;
        }
    }
}
