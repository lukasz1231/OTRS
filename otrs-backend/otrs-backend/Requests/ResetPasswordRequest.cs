using System.ComponentModel.DataAnnotations;

namespace otrs_backend.Requests
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "Hasło musi mieć co najmniej 8 znaków.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
