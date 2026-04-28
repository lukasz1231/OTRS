using Microsoft.EntityFrameworkCore;
using otrs_backend.Data;
using otrs_backend.Models;
using otrs_backend.Requests;
using otrs_backend.Services;
using Xunit;

namespace otrs_backend.Tests.Services
{
    public class TicketQueueRoutingTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly TicketService _service;

        public TicketQueueRoutingTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            _context.Statuses.Add(new Status { Id = 1, Name = "Nowy", Description = "Nowe zgłoszenie" });
            _context.Ques.Add(new Que { Id = 1, Name = "Ogólna" });
            _context.SaveChanges();

            _service = new TicketService(_context);
        }

        [Fact]
        public async Task CreateTicketAsync_BezQueueId_PrzydzielaDomyslnaKolejke()
        {
            var request = new CreateTicketRequest
            {
                Title = "Nowe zgłoszenie",
                Description = "Opis testowy",
                CategoryId = 1,
                PriorityId = 1,
                TypeId = 1,
                QueueId = null
            };

            var ticket = await _service.CreateTicketAsync(request, creatorId: 42);

            Assert.Equal(1, ticket.QueueId);
            Assert.Equal("Nowy", _context.Statuses.Single(s => s.Id == ticket.StatusId).Name);
        }

        [Fact]
        public async Task CreateTicketAsync_ZQueueId_UzywaWskazanejKolejki()
        {
            _context.Ques.Add(new Que { Id = 2, Name = "Techniczna" });
            await _context.SaveChangesAsync();

            var request = new CreateTicketRequest
            {
                Title = "Nowe zgłoszenie",
                Description = "Opis testowy",
                CategoryId = 1,
                PriorityId = 1,
                TypeId = 1,
                QueueId = 2
            };

            var ticket = await _service.CreateTicketAsync(request, creatorId: 42);

            Assert.Equal(2, ticket.QueueId);
        }

        [Fact]
        public async Task UpdateQueueAsync_ZmieniaQueueId_NaIstniejacaKolejke()
        {
            _context.Ques.Add(new Que { Id = 2, Name = "Techniczna" });

            var ticket = new Ticket
            {
                Id = 7,
                PublicId = "PL2026042100007",
                Title = "Test",
                Description = "Opis testowy",
                StatusId = 1,
                CreatedAt = DateTime.UtcNow,
                CreatorId = 1,
                QueueId = 1
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            await _service.UpdateQueueAsync(ticket.Id, 2);

            var updatedTicket = await _context.Tickets.FindAsync(ticket.Id);
            Assert.Equal(2, updatedTicket.QueueId);
        }

        [Fact]
        public async Task UpdateQueueAsync_NieistniejacaKolejka_Wyjatkiem()
        {
            var ticket = new Ticket
            {
                Id = 8,
                PublicId = "PL2026042100008",
                Title = "Test",
                Description = "Opis testowy",
                StatusId = 1,
                CreatedAt = DateTime.UtcNow,
                CreatorId = 1,
                QueueId = 1
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            var exception = await Assert.ThrowsAsync<Exception>(() => _service.UpdateQueueAsync(ticket.Id, 999));

            Assert.Contains("Kolejka o ID '999' nie istnieje", exception.Message);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}