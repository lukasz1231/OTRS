using Microsoft.EntityFrameworkCore;
using otrs_backend.Data;
using otrs_backend.Models;
using otrs_backend.Services;
using Xunit;

namespace otrs_backend.Tests.Services
{
    public class TicketServiceTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly TicketService _service;

        public TicketServiceTests()
            {
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;

                _context = new AppDbContext(options);
                
                _context.Statuses.AddRange(
                    new Status { Id = 1, Name = "Nowy", Description = "Nowe zgłoszenie" },
                    new Status { Id = 2, Name = "W toku", Description = "Zgłoszenie w realizacji" },
                    new Status { Id = 3, Name = "Rozwiązane", Description = "Zgłoszenie rozwiązane" },
                    new Status { Id = 4, Name = "Wstrzymane", Description = "Zgłoszenie wstrzymane" },
                    new Status { Id = 5, Name = "Oczekuje na odpowiedź klienta", Description = "Oczekiwanie na odpowiedź" },
                    new Status { Id = 6, Name = "Wykonane", Description = "Zgłoszenie wykonane" }
                );
                _context.SaveChanges();

                _service = new TicketService(_context);
            }

        // ✅ TEST 1: Dozwolona zmiana Nowy -> W toku
        [Fact]
        public async Task UpdateStatusAsync_NowyNaWToku_Dozwolone_StatusZmieniony()
        {
            // Arrange
            var ticket = new Ticket
            {
                Id = 1,
                PublicId = "PL2026042100001",
                Title = "Test",
                Description = "Opis testowy",
                StatusId = 1, // Nowy
                CreatedAt = DateTime.UtcNow,
                CreatorId = 1
            };
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            // Act
            await _service.UpdateStatusAsync(ticket.Id, 2); // Zmiana na W toku

            // Assert
            var updatedTicket = await _context.Tickets.FindAsync(ticket.Id);
            Assert.Equal(2, updatedTicket.StatusId);
        }

        // ✅ TEST 2: Dozwolona zmiana W toku -> Rozwiązane
        [Fact]
        public async Task UpdateStatusAsync_WTokuNaRozwiazane_Dozwolone_StatusZmieniony()
        {
            // Arrange
            var ticket = new Ticket
            {
                Id = 2,
                PublicId = "PL2026042100002",
                Title = "Test",
                Description = "Opis testowy",
                StatusId = 2, // W toku
                CreatedAt = DateTime.UtcNow,
                CreatorId = 1
            };
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            // Act
            await _service.UpdateStatusAsync(ticket.Id, 3); // Zmiana na Rozwiązane

            // Assert
            var updatedTicket = await _context.Tickets.FindAsync(ticket.Id);
            Assert.Equal(3, updatedTicket.StatusId);
        }

        // ✅ TEST 3: Dozwolona zmiana Rozwiązane -> Wykonane
        [Fact]
        public async Task UpdateStatusAsync_RozwiazaneNaWykonane_Dozwolone_StatusZmieniony()
        {
            // Arrange
            var ticket = new Ticket
            {
                Id = 3,
                PublicId = "PL2026042100003",
                Title = "Test",
                Description = "Opis testowy",
                StatusId = 3, // Rozwiązane
                CreatedAt = DateTime.UtcNow,
                CreatorId = 1
            };
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            // Act
            await _service.UpdateStatusAsync(ticket.Id, 6); // Zmiana na Wykonane

            // Assert
            var updatedTicket = await _context.Tickets.FindAsync(ticket.Id);
            Assert.Equal(6, updatedTicket.StatusId);
        }

        // ❌ TEST 4: Niedozwolona zmiana Nowy -> Wykonane (pomijając pośrednie statusy)
        [Fact]
        public async Task UpdateStatusAsync_NowyNaWykonane_Niedozwolone_Wyjatek()
        {
            // Arrange
            var ticket = new Ticket
            {
                Id = 4,
                PublicId = "PL2026042100004",
                Title = "Test",
                Description = "Opis testowy",
                StatusId = 1, // Nowy
                CreatedAt = DateTime.UtcNow,
                CreatorId = 1
            };
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => 
                _service.UpdateStatusAsync(ticket.Id, 6)); // Próba zmiany na Wykonane

            Assert.Contains("nie jest dozwolone", exception.Message);
        }

        // ❌ TEST 5: Niedozwolona zmiana Wykonane -> Nowy (cofnięcie zakończonego)
        [Fact]
        public async Task UpdateStatusAsync_WykonaneNaNowy_Niedozwolone_Wyjatek()
        {
            // Arrange
            var ticket = new Ticket
            {
                Id = 5,
                PublicId = "PL2026042100005",
                Title = "Test",
                Description = "Opis testowy",
                StatusId = 6, // Wykonane
                CreatedAt = DateTime.UtcNow,
                CreatorId = 1
            };
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => 
                _service.UpdateStatusAsync(ticket.Id, 1)); // Próba zmiany na Nowy

            Assert.Contains("nie jest dozwolone", exception.Message);
        }

        // ✅ TEST 6: Sprawdzenie naliczania czasu pauzy
        [Fact]
        public async Task UpdateStatusAsync_PrzejscieWPauze_ZapisujeCzasPauzy()
        {
            // Arrange
            var ticket = new Ticket
            {
                Id = 6,
                PublicId = "PL2026042100006",
                Title = "Test",
                Description = "Opis testowy",
                StatusId = 2, // W toku
                CreatedAt = DateTime.UtcNow.AddHours(-2),
                CreatorId = 1
            };
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            // Act - zmiana na status pauzy (Wstrzymane)
            await _service.UpdateStatusAsync(ticket.Id, 4);

            // Assert
            var updatedTicket = await _context.Tickets.FindAsync(ticket.Id);
            Assert.Equal(4, updatedTicket.StatusId);
            Assert.NotNull(updatedTicket.PausedAtUtc);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}