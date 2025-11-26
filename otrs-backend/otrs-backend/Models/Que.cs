using System.ComponentModel.DataAnnotations;

namespace otrs_backend.Models
{
    public class Que
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
