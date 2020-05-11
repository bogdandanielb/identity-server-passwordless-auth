using System.Collections.Generic;

namespace DanielBogdan.Passwordless.Identity.Core.Configuration
{
    public class EmailConfiguration
    {
        /// <summary>
        /// Gets or sets the API key.
        /// Used by SendGrid only via SendGridClientOptions
        /// </summary>
        /// <value>
        /// The API key.
        /// </value>
        public string ApiKey { get; set; }

        public string CC { get; set; }
        public string BCC { get; set; }
        public string SupportEmail { get; set; }
    }
}
