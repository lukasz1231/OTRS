using System.ComponentModel.DataAnnotations;

namespace otrs_backend.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } // np. "Hustletrack ITSM"

        public string? Description { get; set; }
        
        // Relacja: Jeden klient może mieć wiele kategorii
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}