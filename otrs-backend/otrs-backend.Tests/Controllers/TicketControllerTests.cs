using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using otrs_backend.Controllers;
using otrs_backend.Data;
using otrs_backend.Models;
using otrs_backend.Services;
using Xunit;
using Type = otrs_backend.Models.Type;

namespace otrs_backend.Tests.Controllers
{
    public class TicketControllerTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly TicketController _controller;

        public TicketControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            SeedCommonData();

            var service = new TicketService(_context);
            _controller = new TicketController(service);
        }

        private void SeedCommonData()
        {
            _context.Roles.AddRange(
                new Role { Id = 1, Name = "User", Description = "Zwykły użytkownik" },
                new Role { Id = 2, Name = "Admin", Description = "Administrator" },
                new Role { Id = 3, Name = "Helpdesk", Description = "Helpdesk" },
                new Role { Id = 4, Name = "Technik", Description = "Technik" });
            _context.Statuses.Add(new Status { Id = 1, Name = "Nowy", Description = "Nowe zgłoszenie" });
            _context.Priorities.Add(new Priority { Id = 1, Name = "Normalny", Description = "Domyślna priorytet", SlaHours = 24 });
            _context.Categories.Add(new Category { Id = 1, Name = "Ogólna", Description = "Domyślna kategoria" });
            _context.Types.Add(new Type { Id = 1, Name = "Zgłoszenie", Description = "Domyślny typ" });
            _context.Ques.Add(new Que { Id = 1, Name = "Ogólna" });
            _context.SaveChanges();
        }

        private void SetUserContext(int userId)
        {
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "test");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        public static IEnumerable<object[]> OtherUserTicketAccessData() => new List<object[]>
        {
            new object[] { "User", false },
            new object[] { "Admin", true },
            new object[] { "Helpdesk", true },
            new object[] { "Technik", false }
        };

        [Theory]
        [MemberData(nameof(OtherUserTicketAccessData))]
        public async Task GetTicketById_OtherUsersTicket_ReturnsExpectedResultForRole(string visitorRoleName, bool shouldHaveAccess)
        {
            var ownerRole = _context.Roles.Single(r => r.Name == "User");
            var visitorRole = _context.Roles.Single(r => r.Name == visitorRoleName);

            var owner = new User
            {
                Id = 1,
                Name = "Owner",
                Surname = "User",
                Email = "owner@example.com",
                PasswordHash = "password",
                Phone = "600000000",
                AvatarUrl = "/avatars/owner.png",
                Bio = "Właściciel zgłoszenia",
                Roles = new List<Role> { ownerRole }
            };

            var visitor = new User
            {
                Id = 2,
                Name = "Visitor",
                Surname = "User",
                Email = "visitor@example.com",
                PasswordHash = "password",
                Phone = "600000001",
                AvatarUrl = "/avatars/visitor.png",
                Bio = "Użytkownik testowy",
                Roles = new List<Role> { visitorRole }
            };

            _context.Users.AddRange(owner, visitor);
            await _context.SaveChangesAsync();

            var ticket = new Ticket
            {
                Id = 1,
                PublicId = "PL202604280001",
                Title = "Prywatne zgłoszenie",
                Description = "Opis prywatny",
                CreatedAt = DateTime.UtcNow,
                CreatorId = owner.Id,
                StatusId = 1,
                PriorityId = 1,
                CategoryId = 1,
                TypeId = 1,
                QueueId = 1
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            SetUserContext(visitor.Id);

            var actionResult = await _controller.GetTicketById(ticket.PublicId);

            if (shouldHaveAccess)
            {
                Assert.IsType<OkObjectResult>(actionResult);
            }
            else
            {
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
                Assert.NotNull(notFoundResult.Value);
                Assert.Contains("nie masz do niego uprawnień", notFoundResult.Value?.ToString() ?? string.Empty, StringComparison.OrdinalIgnoreCase);
            }
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
