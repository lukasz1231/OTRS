using System.ComponentModel.DataAnnotations;

namespace otrs_backend.Requests
{
    public class CreateTicketRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // ZMIANA: z string na int
    public int ClientId { get; set; } 
    public int TypeId { get; set; }
    public int PriorityId { get; set; }
    public int CategoryId { get; set; }
    public int QueueId { get; set; }
}
}