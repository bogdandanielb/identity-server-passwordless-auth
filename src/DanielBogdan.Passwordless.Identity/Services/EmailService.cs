using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DanielBogdan.Passwordless.Identity.Abstractions;
using DanielBogdan.Passwordless.Identity.Core;
using DanielBogdan.Passwordless.Identity.Core.Configuration;
using DanielBogdan.Passwordless.Identity.Email;
using DanielBogdan.Passwordless.Identity.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace DanielBogdan.Passwordless.Identity.Services
{
    /// <summary>
    /// Email service.
    /// </summary>
    /// <seealso cref="IEmailService" />
    public class EmailService : IEmailService
    {
        private readonly ILogger _logger;
        private readonly ISendGridClient _sendGridClient;
        private readonly EmailConfiguration _emailConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailService"/> class.
        /// </summary>
        /// <param name="emailConfiguration">The configuration.</param>
        public EmailService(IOptionsMonitor<EmailConfiguration> emailConfiguration,
                ISendGridClient sendGridClient,
                ILogger<EmailService> logger)
        {
            _logger = logger;
            _sendGridClient = sendGridClient ?? throw new ArgumentNullException(nameof(sendGridClient));
            _emailConfiguration = emailConfiguration.CurrentValue ?? throw new ArgumentNullException(nameof(emailConfiguration));
        }

        /// <summary>
        /// Sends asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// A task returning a boolean indicating
        /// whether or not the send was successful.
        /// </returns>
        public async Task SendAsync(EmailMessage message)
        {
            _logger.LogDebug($"Sending email: {JsonConvert.SerializeObject(message)}");

            var to = message.To.Split(';', StringSplitOptions.RemoveEmptyEntries);
            var cc = string.IsNullOrWhiteSpace(message.CC) ? new List<string>() : message.CC.Split(';', StringSplitOptions.RemoveEmptyEntries).Except(to, StringComparer.OrdinalIgnoreCase).ToList();
            var bcc = string.IsNullOrWhiteSpace(message.BCC) ? new List<string>() : message.BCC.Split(';', StringSplitOptions.RemoveEmptyEntries).Except(to, StringComparer.OrdinalIgnoreCase).ToList();

            var sendGridMessage = new SendGridMessage();

            sendGridMessage.SetFrom(string.IsNullOrWhiteSpace(message.From) ? _emailConfiguration.SupportEmail : message.From);
            sendGridMessage.AddTos(to.Select(t => new EmailAddress(t)).ToList());

            if (cc.Count > 0)
            {
                sendGridMessage.AddCcs(cc.Select(t => new EmailAddress(t)).ToList());
            }
            if (bcc.Count > 0)
            {
                sendGridMessage.AddBccs(bcc.Select(t => new EmailAddress(t)).ToList());
            }

            if (message.Attachments != null && message.Attachments.Count > 0)
            {
                sendGridMessage.AddAttachments(message.Attachments.Select(attachment => new Attachment() { Filename = attachment.Key, Content = attachment.Value.ToBase64() }).ToList());
            }

            if (message is TemplateEmailMessage templateEmailMessage)
            {
                sendGridMessage.SetTemplateId(templateEmailMessage.TemplateId);
                sendGridMessage.SetTemplateData(templateEmailMessage.TemplateData);
            }
            else if (message is HtmlEmailMessage htmlEmailMessage)
            {
                sendGridMessage.SetSubject(htmlEmailMessage.Subject);
                sendGridMessage.AddContent(MimeType.Html, htmlEmailMessage.Body);
            }

            var response = await _sendGridClient.SendEmailAsync(sendGridMessage);

            _logger.LogDebug($"Sendgrid response: {await response.Body?.ReadAsStringAsync()}");

            if (response.StatusCode < HttpStatusCode.OK || response.StatusCode >= HttpStatusCode.Ambiguous)
            {
                throw new PasswordlessException($"Unable to send email : {await response.Body?.ReadAsStringAsync()}");
            }
        }
    }
}
