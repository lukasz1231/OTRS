using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using otrs_backend.Data;
using System.Security.Claims;

namespace otrs_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificationsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications([FromQuery] bool unreadOnly = false)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized();
            }

            var query = _context.Notifications.Where(n => n.UserId == userId);

            if (unreadOnly)
            {
                query = query.Where(n => !n.IsRead);
            }

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Take(50) // Pobieramy maksymalnie 50 najnowszych
                .Select(n => new
                {
                    n.Id,
                    n.Title,
                    n.Message,
                    n.TicketPublicId,
                    n.IsRead,
                    n.CreatedAt
                })
                .ToListAsync();

            return Ok(notifications);
        }

        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized();
            }

            var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
            if (notification == null)
            {
                return NotFound("Powiadomienie nie zostało znalezione.");
            }

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Oznaczono jako przeczytane." });
        }

        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized();
            }

            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            if (unreadNotifications.Any())
            {
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Wszystkie powiadomienia zostały oznaczone jako przeczytane." });
        }
    }
}
