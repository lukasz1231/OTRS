using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace otrs_backend.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        
        [JsonIgnore]
        public User? User { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        // Będziemy zapisywać PublicId (np. PL2026040100001), żeby łatwo nawigować z poziomu UI
        public string? TicketPublicId { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
