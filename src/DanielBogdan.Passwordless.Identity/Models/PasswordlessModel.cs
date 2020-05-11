using System.ComponentModel.DataAnnotations;

namespace DanielBogdan.Passwordless.Identity.Models
{
    public class PasswordlessModel
    {

        [Required]
        public string Link { get; set; }
    }
}
