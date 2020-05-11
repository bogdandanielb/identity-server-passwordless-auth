using System.Collections.Generic;
using System.IO;

namespace DanielBogdan.Passwordless.Identity.Email
{

    /// <summary>
    /// Email message
    /// </summary>
    public abstract class EmailMessage
    {
        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>
        /// From.
        /// </value>
        public string From { get; set; }

        /// <summary>
        /// List of To recipients.
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Gets or sets the cc.
        /// </summary>
        /// <value>
        /// The cc.
        /// </value>
        public string CC { get; set; }

        /// <summary>
        /// Gets or sets the BCC.
        /// </summary>
        /// <value>
        /// The BCC.
        /// </value>
        public string BCC { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets the attachments.
        /// </summary>
        public Dictionary<string, Stream> Attachments { get; set; } = new Dictionary<string, Stream>();

    }
}
