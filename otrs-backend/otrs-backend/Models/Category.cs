using System.ComponentModel.DataAnnotations;

namespace otrs_backend.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int? ClientId { get; set; }
        public Client? Client { get; set; }
    }
}
