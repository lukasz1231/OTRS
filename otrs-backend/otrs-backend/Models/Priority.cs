using System.ComponentModel.DataAnnotations;

namespace otrs_backend.Models
{
    public class Priority
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
    }
}
