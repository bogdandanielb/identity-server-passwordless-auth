using System.Collections.Generic;
using System.IO;

namespace DanielBogdan.Passwordless.Identity.Email
{

    /// <summary>
    /// Email message
    /// </summary>
    public class TemplateEmailMessage: EmailMessage
    {
        /// <summary>
        /// Gets the template identifier.
        /// Used for https://sendgrid.com/docs/ui/sending-email/how-to-send-an-email-with-dynamic-transactional-templates/
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public string TemplateId { get; set; }

        /// <summary>
        /// Gets the dynamic template data.
        /// Used for https://sendgrid.com/docs/ui/sending-email/how-to-send-an-email-with-dynamic-transactional-templates/
        /// </summary>
        public dynamic TemplateData { get; set; }

    }
}
