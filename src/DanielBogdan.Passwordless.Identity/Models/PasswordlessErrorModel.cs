using System.ComponentModel.DataAnnotations;

namespace DanielBogdan.Passwordless.Identity.Models
{
    public class PasswordlessErrorModel
    {
        [Required]
        public string Message { get; set; }
    }
}
