using Microsoft.EntityFrameworkCore;
using otrs_backend.Data;
using otrs_backend.Models;
using otrs_backend.Requests;

namespace otrs_backend.Services
{
    public class TicketDto
    {
        public int Id { get; set; }
        // DODANE: Pole dla PublicId (jak w Twoich mockach)
        public string PublicId => $"ZGL-{Id:D5}";
        public string Title { get; set; }
        public string Description { get; set; } // DODANE: Opis
        public DateTime CreatedAt { get; set; }
        public string Client { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string Queue { get; set; }
        public bool IsMyTicket { get; set; }
    }

    public class TicketService
    {
        private readonly AppDbContext _context;

        public TicketService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Ticket> CreateTicketAsync(CreateTicketRequest request, int creatorId)
        {
            var initialStatus = await _context.Statuses
                .FirstOrDefaultAsync(s => s.Name == "Nowy")
                ?? throw new Exception("Błąd konfiguracji systemu: Brak statusu 'Nowy' w bazie danych.");

            var ticket = new Ticket
            {
                Title = request.Title,
                Description = request.Description,
                Client = request.Client,
                CategoryId = request.CategoryId,
                PriorityId = request.PriorityId,
                TypeId = request.TypeId,
                QueueId = request.QueueId,
                CreatorId = creatorId,
                StatusId = initialStatus.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return ticket;
        }

        public async Task<List<TicketDto>> GetMyTicketsAsync(int currentUserId)
        {
            return await _context.Tickets
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Include(t => t.Category)
                .Include(t => t.Type)
                .Include(t => t.Queue)
                .Where(t => t.CreatorId == currentUserId || t.AssignedUsers.Any(u => u.Id == currentUserId))
                .Select(t => new TicketDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description, // ZMIANA: Przesyłamy opis z bazy
                    CreatedAt = t.CreatedAt,
                    Client = t.Client,
                    Status = t.Status.Name,
                    Priority = t.Priority.Name,
                    Category = t.Category.Name,
                    Type = t.Type.Name,
                    Queue = t.Queue.Name,
                    IsMyTicket = t.CreatorId == currentUserId
                })
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}