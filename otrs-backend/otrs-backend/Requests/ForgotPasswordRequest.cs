using System.ComponentModel.DataAnnotations;

namespace otrs_backend.Requests
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
