using Microsoft.EntityFrameworkCore;
using otrs_backend.Data;
using otrs_backend.Models;
using otrs_backend.Requests;

namespace otrs_backend.Services
{
    public class AttachmentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = default!;
        public string FilePath { get; set; } = default!;
    }

    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; } = default!;
        public string UserRole { get; set; } = default!;
        public List<AttachmentDto> Attachments { get; set; } = new();
    }

    public class TicketDto
    {
        public int Id { get; set; }
        public string PublicId { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public string Client { get; set; } = default!; // Zwracamy nazwę klienta jako string do frontu
        public string Status { get; set; } = default!;
        public string Priority { get; set; } = default!;
        public string Category { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Queue { get; set; } = default!;
        public bool IsMyTicket { get; set; }
        public List<CommentDto> Comments { get; set; } = new();
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

            var publicId = await GeneratePublicIdAsync();

            var ticket = new Ticket
            {
                PublicId = publicId,
                Title = request.Title,
                Description = request.Description,
                ClientId = request.ClientId, // POPRAWKA: używamy ID z modelu Client
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

        private async Task<string> GeneratePublicIdAsync()
        {
            var today = DateTime.UtcNow.ToString("yyyyMMdd");
            var prefix = "PL";

            var lastTicket = await _context.Tickets
                .Where(t => t.PublicId != null && t.PublicId.StartsWith(prefix + today))
                .OrderByDescending(t => t.PublicId)
                .FirstOrDefaultAsync();

            if (lastTicket == null)
            {
                return $"{prefix}{today}00001";
            }

            var lastNumberStr = lastTicket.PublicId.Substring(prefix.Length + today.Length);
            if (int.TryParse(lastNumberStr, out int lastNumber))
            {
                var newNumber = lastNumber + 1;
                return $"{prefix}{today}{newNumber:D5}";
            }

            return $"{prefix}{today}00001";
        }

        public async Task<List<TicketDto>> GetMyTicketsAsync(int currentUserId)
        {
            var userRoles = await GetUserRolesAsync(currentUserId);

            var visibleTicketsQuery = ApplyTicketVisibilityRules(_context.Tickets, currentUserId, userRoles);

            return await visibleTicketsQuery
                .Include(t => t.Client) // WAŻNE: dociągamy dane klienta
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Include(t => t.Category)
                .Include(t => t.Type)
                .Include(t => t.Queue)
                .Select(t => new TicketDto
                {
                    Id = t.Id,
                    PublicId = t.PublicId,
                    Title = t.Title,
                    Description = t.Description,
                    CreatedAt = t.CreatedAt,
                    Client = t.Client != null ? t.Client.Name : "Brak klienta", // Pobieramy nazwę z relacji
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

        public async Task<TicketDto?> GetTicketByIdAsync(int ticketId, int currentUserId)
        {
            var userRoles = await GetUserRolesAsync(currentUserId);

            var visibleTicketsQuery = ApplyTicketVisibilityRules(_context.Tickets, currentUserId, userRoles);

            return await visibleTicketsQuery
                .Include(t => t.Client) // WAŻNE: dociągamy dane klienta
                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.Attachments)
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Include(t => t.Category)
                .Include(t => t.Type)
                .Include(t => t.Queue)
                .Where(t => t.Id == ticketId)
                .Select(t => new TicketDto
                {
                    Id = t.Id,
                    PublicId = t.PublicId,
                    Title = t.Title,
                    Description = t.Description,
                    CreatedAt = t.CreatedAt,
                    Client = t.Client != null ? t.Client.Name : "Brak klienta", // Pobieramy nazwę z relacji
                    Status = t.Status.Name,
                    Priority = t.Priority.Name,
                    Category = t.Category.Name,
                    Type = t.Type.Name,
                    Queue = t.Queue.Name,
                    IsMyTicket = t.CreatorId == currentUserId,

                    Comments = t.Comments.Select(c => new CommentDto
                    {
                        Id = c.Id,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt,
                        UserName = $"{c.User.Name} {c.User.Surname}",
                        UserRole = string.Join(", ", c.User.Roles.Select(r => r.Name)),

                        Attachments = c.Attachments.Select(a => new AttachmentDto
                        {
                            Id = a.Id,
                            FileName = a.FileName,
                            FilePath = a.FilePath
                        }).ToList()
                    })
                    .OrderBy(c => c.CreatedAt)
                    .ToList()
                })
                .FirstOrDefaultAsync();
        }

        private async Task<HashSet<string>> GetUserRolesAsync(int userId)
        {
            var roles = await _context.Users
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Roles.Select(r => r.Name))
                .ToListAsync();

            return roles.ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        private static IQueryable<Ticket> ApplyTicketVisibilityRules(
            IQueryable<Ticket> query,
            int currentUserId,
            HashSet<string> userRoles)
        {
            // Admin i Helpdesk widzą wszystkie zgłoszenia
            if (userRoles.Contains("Admin") || userRoles.Contains("Helpdesk"))
            {
                return query;
            }

            // Technik widzi zgłoszenia przypisane do niego lub znajdujące się w jego kolejkach
            if (userRoles.Contains("Technik"))
            {
                return query.Where(t =>
                    t.AssignedUsers.Any(u => u.Id == currentUserId) ||
                    t.Queue.Users.Any(u => u.Id == currentUserId));
            }

            // Zwykły użytkownik widzi własne zgłoszenia oraz te przypisane do niego.
            return query.Where(t =>
                t.CreatorId == currentUserId ||
                t.AssignedUsers.Any(u => u.Id == currentUserId));
        }

        public async Task AddCommentAsync(int ticketId, int userId, string content, IFormFileCollection files)
        {
            var comment = new Comment
            {
                TicketId = ticketId,
                UserId = userId,
                Content = content ?? "",
                CreatedAt = DateTime.Now
            };

            if (files != null && files.Count > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                foreach (var file in files)
                {
                    var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    comment.Attachments.Add(new otrs_backend.Models.Attachment
                    {
                        FileName = file.FileName,
                        FilePath = $"/uploads/{uniqueFileName}",
                        ContentType = file.ContentType,
                        FileSize = file.Length
                    });
                }
            }

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int ticketId, int newStatusId) // POPRAWKA: przyjmujemy int Id
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
            if (ticket == null)
            {
                throw new Exception($"Nie znaleziono zgłoszenia o ID: {ticketId}");
            }

            var statusExists = await _context.Statuses.AnyAsync(s => s.Id == newStatusId);
            if (!statusExists)
            {
                throw new Exception($"Status o ID '{newStatusId}' nie istnieje.");
            }

            ticket.StatusId = newStatusId;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Status>> GetAllStatusesAsync()
        {
            return await _context.Statuses.ToListAsync();
        }
    }
}