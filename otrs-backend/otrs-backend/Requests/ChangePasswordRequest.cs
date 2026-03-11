using System.ComponentModel.DataAnnotations;

namespace otrs_backend.Requests
{
    public class ChangePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "Haslo musi miec co najmniej 8 znakow.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}