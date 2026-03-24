using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using otrs_backend.Data;
using otrs_backend.Models;

namespace otrs_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // ZMIANA: Teraz cały kontroler jest dostępny dla Admina i Helpdesku (pobieranie danych)
    [Authorize(Roles = "Admin,Helpdesk")] 
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
        [Authorize(Roles = "Admin")] // Tylko Admin edytuje użytkowników
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
        [AllowAnonymous]
        public async Task<IActionResult> GetQueues()
        {
            return Ok(await _context.Ques.Select(q => new { q.Id, q.Name, UserCount = q.Users.Count }).ToListAsync());
        }

        [HttpPost("queues")]
        [Authorize(Roles = "Admin")] // Tylko Admin tworzy kolejki
        public async Task<IActionResult> CreateQueue([FromBody] CreateQueueRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) return BadRequest("Nazwa jest wymagana.");
            var queue = new Que { Name = request.Name };
            _context.Ques.Add(queue);
            await _context.SaveChangesAsync();
            return Ok(queue);
        }

        [HttpDelete("queues/{id}")]
        [Authorize(Roles = "Admin")] // Tylko Admin usuwa kolejki
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
            var queue = await _context.Ques
                .Include(q => q.Users)
                    .ThenInclude(u => u.Roles) 
                .FirstOrDefaultAsync(q => q.Id == id);

            if (queue == null) return NotFound();

            var result = queue.Users.Select(u => new { 
                u.Id, 
                u.Name, 
                u.Surname, 
                u.Email,
                Roles = u.Roles.Select(r => r.Name).ToList()
            });

            return Ok(result);
        }

        [HttpPost("queues/{id}/users/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUserToQueue(int id, int userId)
        {
            var queue = await _context.Ques.Include(q => q.Users).FirstOrDefaultAsync(q => q.Id == id);
            var user = await _context.Users.FindAsync(userId);
            if (queue == null || user == null) return NotFound();
            if (!queue.Users.Any(u => u.Id == userId)) { queue.Users.Add(user); await _context.SaveChangesAsync(); }
            return Ok();
        }

        [HttpDelete("queues/{id}/users/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveUserFromQueue(int id, int userId)
        {
            var queue = await _context.Ques.Include(q => q.Users).FirstOrDefaultAsync(q => q.Id == id);
            var user = queue?.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null) { queue.Users.Remove(user); await _context.SaveChangesAsync(); }
            return Ok();
        }

        #endregion

        #region Zarządzanie Statusami

        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatuses() 
        {
            return Ok(await _context.Statuses.ToListAsync());
        }

        [HttpPost("statuses")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateStatus([FromBody] Status status)
        {
            if (string.IsNullOrWhiteSpace(status.Name)) return BadRequest("Nazwa jest wymagana.");
            _context.Statuses.Add(status);
            await _context.SaveChangesAsync();
            return Ok(status);
        }

        [HttpDelete("statuses/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStatus(int id)
        {
            var status = await _context.Statuses.FindAsync(id);
            if (status == null) return NotFound();
            var isUsed = await _context.Tickets.AnyAsync(t => t.StatusId == id);
            if (isUsed) return BadRequest("Nie można usunąć statusu, który jest przypisany do zgłoszeń!");
            _context.Statuses.Remove(status);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("statuses/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] Status status)
        {
            var existingStatus = await _context.Statuses.FindAsync(id);
            if (existingStatus == null) return NotFound();
            existingStatus.Name = status.Name;
            existingStatus.Description = status.Description;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Status zaktualizowany" });
        }

        #endregion

        #region Zarządzanie Kategoriami (Services)

        [HttpGet("categories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories() => Ok(await _context.Categories.ToListAsync());

        [HttpGet("categories-all")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _context.Categories
                .Select(c => new {
                    c.Id,
                    c.Name,
                    c.Description,
                    c.ClientId 
                })
                .ToListAsync();
            return Ok(categories);
        }

        [HttpPost("categories")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] Category cat)
        {
            if (string.IsNullOrWhiteSpace(cat.Name)) 
                return BadRequest(new { message = "Nazwa jest wymagana" });
            cat.Client = null; 
            _context.Categories.Add(cat);
            await _context.SaveChangesAsync();
            return Ok(cat);
        }

        [HttpPut("categories/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category cat)
        {
            var existing = await _context.Categories.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Name = cat.Name;
            existing.Description = cat.Description;
            existing.ClientId = cat.ClientId;
            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("categories/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var cat = await _context.Categories.FindAsync(id);
            if (cat == null) return NotFound();
            if (await _context.Tickets.AnyAsync(t => t.CategoryId == id)) 
                return BadRequest("Kategoria jest przypisana do zgłoszeń!");
            _context.Categories.Remove(cat);
            await _context.SaveChangesAsync();
            return Ok();
        }

        #endregion

        #region Zarządzanie Priorytetami

        [HttpGet("priorities")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPriorities() => Ok(await _context.Priorities.OrderByDescending(p => p.Level).ToListAsync());

        [HttpPost("priorities")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePriority([FromBody] Priority prio)
        {
            _context.Priorities.Add(prio);
            await _context.SaveChangesAsync();
            return Ok(prio);
        }

        [HttpPut("priorities/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePriority(int id, [FromBody] Priority prio)
        {
            var existing = await _context.Priorities.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Name = prio.Name;
            existing.Description = prio.Description;
            existing.Level = prio.Level;
            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("priorities/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePriority(int id)
        {
            var prio = await _context.Priorities.FindAsync(id);
            if (prio == null) return NotFound();
            if (await _context.Tickets.AnyAsync(t => t.PriorityId == id)) 
                return BadRequest("Priorytet jest używany!");
            _context.Priorities.Remove(prio);
            await _context.SaveChangesAsync();
            return Ok();
        }

        #endregion

        #region Zarządzanie Klientami

        [HttpGet("clients")]
        [AllowAnonymous]
        public async Task<IActionResult> GetClients() => Ok(await _context.Clients.ToListAsync());

        [HttpPost("clients")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateClient([FromBody] Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return Ok(client);
        }

        [HttpPut("clients/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateClient(int id, [FromBody] Client client)
        {
            var existing = await _context.Clients.FindAsync(id);
            if (existing == null) return NotFound();
            existing.Name = client.Name;
            existing.Description = client.Description;
            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("clients/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return Ok();
        }

        #endregion

        [HttpGet("types")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTypes() 
        {
            return Ok(await _context.Types.ToListAsync());
        }
    }

    public class UpdateUserRequest { public string Name { get; set; } = string.Empty; public string Surname { get; set; } = string.Empty; public string Email { get; set; } = string.Empty; public string? NewPassword { get; set; } public List<string> Roles { get; set; } = new List<string>(); }
    public class CreateQueueRequest { public string Name { get; set; } = string.Empty; }
}