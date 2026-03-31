using System.ComponentModel.DataAnnotations;

namespace otrs_backend.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime BirthDate { get; set; }
        public string Bio { get; set; }
        public string AvatarUrl { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Phone(ErrorMessage = "Niepoprawny format telefonu")]
        public string? Phone { get; set; }
        [Required]
        public string PasswordHash { get; set; }

        public int? ClientId { get; set; }
        public Client? Client { get; set; }

        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }

        public ICollection<Role> Roles { get; set; } = new List<Role>();
        public ICollection<Que> Ques { get; set; } = new List<Que>();
        public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();
        public ICollection<Ticket> CreatedTickets { get; set; } = new List<Ticket>();
    }
}
