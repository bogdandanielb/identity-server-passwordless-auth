using System.Collections.Generic;
using System.IO;

namespace DanielBogdan.Passwordless.Identity.Email
{

    /// <summary>
    /// Email message
    /// </summary>
    public class HtmlEmailMessage: EmailMessage
    {
        /// <summary>
        /// Gets or sets the HTML.
        /// </summary>
        public string Body { get; set; }
    }
}
