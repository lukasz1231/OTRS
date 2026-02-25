using System.ComponentModel.DataAnnotations;

namespace otrs_backend.Requests
{
    public class CreateTicketRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Title { get; set; }

        [Required]
        [MinLength(20)]
        public string Description { get; set; }

        [Required]
        public string Client { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int PriorityId { get; set; }

        [Required]
        public int TypeId { get; set; }

        [Required]
        public int QueueId { get; set; }
    }
}