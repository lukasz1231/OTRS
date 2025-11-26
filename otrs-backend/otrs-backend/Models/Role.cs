using System.ComponentModel.DataAnnotations;

namespace otrs_backend.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
