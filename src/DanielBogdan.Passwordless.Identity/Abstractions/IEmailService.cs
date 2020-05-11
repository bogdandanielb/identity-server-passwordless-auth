using System.Threading.Tasks;
using DanielBogdan.Passwordless.Identity.Email;

namespace DanielBogdan.Passwordless.Identity.Abstractions
{
    /// <summary>
    /// Email service interface.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// A task returning a boolean indicating
        /// whether or not the send was successful.
        /// </returns>
        Task SendAsync(EmailMessage message);
    }
}
