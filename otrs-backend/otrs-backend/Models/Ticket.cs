using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;

namespace otrs_backend.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public int CreatorId { get; set; }
        public User Creator { get; set; }
        public string Client { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int PriorityId { get; set; }
        public Priority Priority { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }
        public int TypeId { get; set; }
        public Type Type { get; set; }
        public int QueueId { get; set; }
        public Que Queue { get; set; }
        public ICollection<User> AssignedUsers { get; set; } = new List<User>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    }
}
