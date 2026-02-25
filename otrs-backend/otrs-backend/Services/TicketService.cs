using Microsoft.EntityFrameworkCore;
using otrs_backend.Data;
using otrs_backend.Models;
using otrs_backend.Requests;

namespace otrs_backend.Services
{

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
    }
}