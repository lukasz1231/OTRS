using System.ComponentModel.DataAnnotations;

namespace otrs_backend.Requests
{
    public class RegisterRequest
    {
        [Required]
        public string Fullname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
