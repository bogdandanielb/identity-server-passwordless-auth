using IdentityServer4.Models;

namespace DanielBogdan.Passwordless.Identity.Models
{
    public class ErrorViewModel
    {
        public int? StatusCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public ErrorMessage IdentityError { get; set; }

    }
}
