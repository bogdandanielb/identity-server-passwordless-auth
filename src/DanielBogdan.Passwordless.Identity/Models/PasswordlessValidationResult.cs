namespace DanielBogdan.Passwordless.Identity.Models
{
    public class PasswordlessValidationResult
    {
        public ApplicationUser User { get; set; }
        public string Token { get; set; }
    }
}
