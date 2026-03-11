using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using otrs_backend.Data;
using otrs_backend.Models;

namespace otrs_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        #region Zarządzanie Użytkownikami

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] string? search)
        {
            var query = _context.Users.Include(u => u.Roles).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(u => 
                    u.Email.ToLower().Contains(search) || 
                    u.Name.ToLower().Contains(search) || 
                    u.Surname.ToLower().Contains(search));
            }

            var users = await query.Select(u => new {
                u.Id,
                u.Name,
                u.Surname,
                u.Email,
                Roles = u.Roles.Select(r => r.Name).ToList()
            }).ToListAsync();

            return Ok(users);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles() => Ok(await _context.Roles.Select(r => r.Name).ToListAsync());

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            var user = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            user.Name = request.Name;
            user.Surname = request.Surname;
            user.Email = request.Email;

            if (!string.IsNullOrWhiteSpace(request.NewPassword))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            if (request.Roles != null)
            {
                user.Roles.Clear();
                var selectedRoles = await _context.Roles.Where(r => request.Roles.Contains(r.Name)).ToListAsync();
                foreach (var role in selectedRoles) user.Roles.Add(role);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Użytkownik zaktualizowany" });
        }

        #endregion

        #region Zarządzanie Kolejkami (Queues)

        [HttpGet("queues")]
        public async Task<IActionResult> GetQueues()
        {
            return Ok(await _context.Ques.Select(q => new { q.Id, q.Name, UserCount = q.Users.Count }).ToListAsync());
        }

        [HttpPost("queues")]
        public async Task<IActionResult> CreateQueue([FromBody] CreateQueueRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) return BadRequest("Nazwa jest wymagana.");
            var queue = new Que { Name = request.Name };
            _context.Ques.Add(queue);
            await _context.SaveChangesAsync();
            return Ok(queue);
        }

        [HttpDelete("queues/{id}")]
        public async Task<IActionResult> DeleteQueue(int id)
        {
            var queue = await _context.Ques.FindAsync(id);
            if (queue == null) return NotFound();
            if (await _context.Tickets.AnyAsync(t => t.QueueId == id)) return BadRequest("Kolejka zawiera zgłoszenia!");
            _context.Ques.Remove(queue);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("queues/{id}/users")]
        public async Task<IActionResult> GetQueueUsers(int id)
        {
            var queue = await _context.Ques.Include(q => q.Users).FirstOrDefaultAsync(q => q.Id == id);
            return queue == null ? NotFound() : Ok(queue.Users.Select(u => new { u.Id, u.Name, u.Surname, u.Email }));
        }

        [HttpPost("queues/{id}/users/{userId}")]
        public async Task<IActionResult> AddUserToQueue(int id, int userId)
        {
            var queue = await _context.Ques.Include(q => q.Users).FirstOrDefaultAsync(q => q.Id == id);
            var user = await _context.Users.FindAsync(userId);
            if (queue == null || user == null) return NotFound();
            if (!queue.Users.Any(u => u.Id == userId)) { queue.Users.Add(user); await _context.SaveChangesAsync(); }
            return Ok();
        }

        [HttpDelete("queues/{id}/users/{userId}")]
        public async Task<IActionResult> RemoveUserFromQueue(int id, int userId)
        {
            var queue = await _context.Ques.Include(q => q.Users).FirstOrDefaultAsync(q => q.Id == id);
            var user = queue?.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null) { queue.Users.Remove(user); await _context.SaveChangesAsync(); }
            return Ok();
        }

        #endregion
    }

    public class UpdateUserRequest { public string Name { get; set; } = string.Empty; public string Surname { get; set; } = string.Empty; public string Email { get; set; } = string.Empty; public string? NewPassword { get; set; } public List<string> Roles { get; set; } = new List<string>(); }
    public class CreateQueueRequest { public string Name { get; set; } = string.Empty; }
}