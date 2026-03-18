using System.ComponentModel.DataAnnotations;

namespace otrs_backend.Requests
{
    public class UpdateProfileRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Surname { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        public string? Bio { get; set; }

        public string? AvatarUrl { get; set; }
    }
}
