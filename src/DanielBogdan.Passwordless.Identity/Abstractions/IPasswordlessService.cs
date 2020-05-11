using DanielBogdan.Passwordless.Identity.Models;
using System.Threading.Tasks;

namespace DanielBogdan.Passwordless.Identity.Abstractions
{
    public interface IPasswordlessService
    {
        Task SendAsync(string clientId, string userId, string url);
        Task<PasswordlessValidationResult> ValidateAsync(string userId, string token);
    }
}
