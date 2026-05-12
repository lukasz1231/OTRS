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
        public string CreatorName { get; set; } = default!;
        public string? CreatorEmail { get; set; }
        public string? CreatorPhone { get; set; }
        public string? ReporterClientName { get; set; }
        public string? ReporterClientPhone { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Client { get; set; } = default!; // Zwracamy nazwę klienta jako string do frontu
        public int? ClientId { get; set; }
        public string Status { get; set; } = default!;
        public int StatusId { get; set; }
        public string Priority { get; set; } = default!;
        public int PriorityId { get; set; }
        public int PrioritySlaHours { get; set; }
        public string Category { get; set; } = default!;
        public int CategoryId { get; set; }
        public string Type { get; set; } = default!;
        public int TypeId { get; set; }
        public string Queue { get; set; } = default!;
        public int QueueId { get; set; }
        public DateTime DueAtUtc { get; set; }
        public DateTime? ResolvedAtUtc { get; set; }
        public DateTime? PausedAtUtc { get; set; }
        public int TotalPausedMinutes { get; set; }
        public int RemainingMinutes { get; set; }
        public bool IsSlaBreached { get; set; }
        public string SlaState { get; set; } = "ok";
        public string SlaMessage { get; set; } = string.Empty;
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

            var queueId = request.QueueId > 0
                ? request.QueueId.Value
                : (await _context.Ques.FirstOrDefaultAsync())?.Id
                  ?? throw new Exception("Brak dostępnych kolejek w systemie.");

            var ticket = new Ticket
            {
                PublicId = publicId,
                Title = request.Title,
                Description = request.Description,
                ClientId = request.ClientId > 0 ? request.ClientId : null,
                CategoryId = request.CategoryId,
                PriorityId = request.PriorityId,
                TypeId = request.TypeId,
                QueueId = queueId,
                CreatorId = creatorId,
                StatusId = initialStatus.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            await NotifyUsersAboutTicketAsync(ticket, "Nowe zgłoszenie", $"Utworzono zgłoszenie: {ticket.Title}", creatorId);

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
            var canViewContactData = userRoles.Contains("Admin") || userRoles.Contains("Helpdesk") || userRoles.Contains("Technik");

            var visibleTicketsQuery = ApplyTicketVisibilityRules(_context.Tickets, currentUserId, userRoles);

            var tickets = await visibleTicketsQuery
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
                    CreatorName = t.Creator.Name + " " + t.Creator.Surname,
                    CreatorEmail = canViewContactData ? t.Creator.Email : null,
                    CreatorPhone = canViewContactData ? t.Creator.Phone : null,
                    ReporterClientName = t.Creator.Client != null ? t.Creator.Client.Name : null,
                    ReporterClientPhone = canViewContactData && t.Creator.Client != null ? t.Creator.Client.Phone : null,
                    CreatedAt = t.CreatedAt,
                    Client = t.Client != null ? t.Client.Name : "Brak klienta", // Pobieramy nazwę z relacji
                    ClientId = t.ClientId,
                    Status = t.Status.Name,
                    StatusId = t.StatusId,
                    Priority = t.Priority.Name,
                    PriorityId = t.PriorityId,
                    PrioritySlaHours = t.Priority.SlaHours,
                    Category = t.Category.Name,
                    CategoryId = t.CategoryId,
                    Type = t.Type.Name,
                    TypeId = t.TypeId,
                    Queue = t.Queue.Name,
                    QueueId = t.QueueId,
                    ResolvedAtUtc = t.ResolvedAtUtc,
                    PausedAtUtc = t.PausedAtUtc,
                    TotalPausedMinutes = t.TotalPausedMinutes,
                    IsMyTicket = t.CreatorId == currentUserId
                })
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            var now = DateTime.UtcNow;
            foreach (var ticket in tickets)
            {
                ApplySla(ticket, now);
            }

            return tickets;
        }

        public async Task<TicketDto?> GetTicketByIdAsync(int ticketId, int currentUserId)
        {
            var userRoles = await GetUserRolesAsync(currentUserId);
            var canViewContactData = userRoles.Contains("Admin") || userRoles.Contains("Helpdesk") || userRoles.Contains("Technik");

            var visibleTicketsQuery = ApplyTicketVisibilityRules(_context.Tickets, currentUserId, userRoles);

            var ticket = await visibleTicketsQuery
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
                    CreatorName = t.Creator.Name + " " + t.Creator.Surname,
                    CreatorEmail = canViewContactData ? t.Creator.Email : null,
                    CreatorPhone = canViewContactData ? t.Creator.Phone : null,
                    ReporterClientName = t.Creator.Client != null ? t.Creator.Client.Name : null,
                    ReporterClientPhone = canViewContactData && t.Creator.Client != null ? t.Creator.Client.Phone : null,
                    CreatedAt = t.CreatedAt,
                    Client = t.Client != null ? t.Client.Name : "Brak klienta", // Pobieramy nazwę z relacji
                    ClientId = t.ClientId,
                    Status = t.Status.Name,
                    StatusId = t.StatusId,
                    Priority = t.Priority.Name,
                    PriorityId = t.PriorityId,
                    PrioritySlaHours = t.Priority.SlaHours,
                    Category = t.Category.Name,
                    CategoryId = t.CategoryId,
                    Type = t.Type.Name,
                    TypeId = t.TypeId,
                    Queue = t.Queue.Name,
                    QueueId = t.QueueId,
                    ResolvedAtUtc = t.ResolvedAtUtc,
                    PausedAtUtc = t.PausedAtUtc,
                    TotalPausedMinutes = t.TotalPausedMinutes,
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

            if (ticket != null)
            {
                ApplySla(ticket, DateTime.UtcNow);
            }

            return ticket;
        }

        public async Task<TicketDto?> GetTicketByPublicIdAsync(string publicId, int currentUserId)
        {
            var userRoles = await GetUserRolesAsync(currentUserId);
            var canViewContactData = userRoles.Contains("Admin") || userRoles.Contains("Helpdesk") || userRoles.Contains("Technik");

            var visibleTicketsQuery = ApplyTicketVisibilityRules(_context.Tickets, currentUserId, userRoles);

            var ticket = await visibleTicketsQuery
                .Include(t => t.Client)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.Attachments)
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Include(t => t.Category)
                .Include(t => t.Type)
                .Include(t => t.Queue)
                .Where(t => t.PublicId == publicId)
                .Select(t => new TicketDto
                {
                    Id = t.Id,
                    PublicId = t.PublicId,
                    Title = t.Title,
                    Description = t.Description,
                    CreatorName = t.Creator.Name + " " + t.Creator.Surname,
                    CreatorEmail = canViewContactData ? t.Creator.Email : null,
                    CreatorPhone = canViewContactData ? t.Creator.Phone : null,
                    ReporterClientName = t.Creator.Client != null ? t.Creator.Client.Name : null,
                    ReporterClientPhone = canViewContactData && t.Creator.Client != null ? t.Creator.Client.Phone : null,
                    CreatedAt = t.CreatedAt,
                    Client = t.Client != null ? t.Client.Name : "Brak klienta",
                    ClientId = t.ClientId,
                    Status = t.Status.Name,
                    StatusId = t.StatusId,
                    Priority = t.Priority.Name,
                    PriorityId = t.PriorityId,
                    PrioritySlaHours = t.Priority.SlaHours,
                    Category = t.Category.Name,
                    CategoryId = t.CategoryId,
                    Type = t.Type.Name,
                    TypeId = t.TypeId,
                    Queue = t.Queue.Name,
                    QueueId = t.QueueId,
                    ResolvedAtUtc = t.ResolvedAtUtc,
                    PausedAtUtc = t.PausedAtUtc,
                    TotalPausedMinutes = t.TotalPausedMinutes,
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

            if (ticket != null)
            {
                ApplySla(ticket, DateTime.UtcNow);
            }

            return ticket;
        }

        public async Task<int?> GetTicketIdByPublicIdAsync(string publicId)
        {
            var ticket = await _context.Tickets
                .Where(t => t.PublicId == publicId)
                .Select(t => new { t.Id })
                .FirstOrDefaultAsync();
            return ticket?.Id;
        }

        private static void ApplySla(TicketDto ticket, DateTime nowUtc)
        {
            ticket.DueAtUtc = ticket.CreatedAt
                .AddHours(ticket.PrioritySlaHours)
                .AddMinutes(ticket.TotalPausedMinutes);

            var isClosed = IsClosedStatus(ticket.Status);
            var isPaused = IsPausedStatus(ticket.Status);

            var referenceTime = isClosed && ticket.ResolvedAtUtc.HasValue
                ? ticket.ResolvedAtUtc.Value
                : isPaused && ticket.PausedAtUtc.HasValue
                    ? ticket.PausedAtUtc.Value
                : nowUtc;

            ticket.RemainingMinutes = (int)Math.Floor((ticket.DueAtUtc - referenceTime).TotalMinutes);
            ticket.IsSlaBreached = ticket.RemainingMinutes < 0;

            if (isClosed)
            {
                ticket.SlaState = ticket.IsSlaBreached ? "breached" : "ok";
                ticket.SlaMessage = ticket.IsSlaBreached
                    ? "Zgłoszenie rozwiązane po SLA"
                    : "Zgłoszenie rozwiązane przed SLA";
                return;
            }

            if (isPaused)
            {
                ticket.SlaState = "paused";
                ticket.SlaMessage = GetPausedSlaMessage(ticket.Status);
                return;
            }

            if (ticket.IsSlaBreached)
            {
                ticket.SlaState = "breached";
                ticket.SlaMessage = "SLA przekroczone";
                return;
            }

            if (ticket.RemainingMinutes <= 120)
            {
                ticket.SlaState = "critical";
                ticket.SlaMessage = "SLA krytyczne (<= 2h)";
                return;
            }

            if (ticket.RemainingMinutes <= 480)
            {
                ticket.SlaState = "warning";
                ticket.SlaMessage = "Uwaga: SLA poniżej 8h";
                return;
            }

            ticket.SlaState = "ok";
            ticket.SlaMessage = "SLA w normie";
        }

        private static bool IsClosedStatus(string status)
        {
            var normalized = NormalizeStatus(status);
            if (string.IsNullOrEmpty(normalized)) return false;

            return normalized.Contains("rozwiaz") || normalized.Contains("zamkniet") || normalized.Contains("wykonan");
        }

        private static bool IsPausedStatus(string status)
        {
            var normalized = NormalizeStatus(status);
            if (string.IsNullOrEmpty(normalized)) return false;

            return normalized.Contains("wstrzym") || normalized.Contains("oczekuje na odpowiedz klienta");
        }

        private static string NormalizeStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status)) return string.Empty;

            return status
                .Normalize(System.Text.NormalizationForm.FormD)
                .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                .Aggregate(string.Empty, (current, c) => current + char.ToLowerInvariant(c))
                .Trim();
        }

        private static string GetPausedSlaMessage(string status)
        {
            var normalized = NormalizeStatus(status);
            if (normalized.Contains("oczekuje na odpowiedz klienta"))
            {
                return "SLA wstrzymane: oczekiwanie na odpowiedź klienta";
            }

            return "SLA wstrzymane: status Wstrzymane";
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

            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
            if (ticket != null)
            {
                await NotifyUsersAboutTicketAsync(ticket, "Nowy komentarz", $"Dodano nowy komentarz w zgłoszeniu: {ticket.Title}", userId);
            }
        }

        public async Task UpdateStatusAsync(int ticketId, int newStatusId, int currentUserId = 0)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
            if (ticket == null)
            {
                throw new Exception($"Nie znaleziono zgłoszenia o ID: {ticketId}");
            }

            var newStatus = await _context.Statuses.FirstOrDefaultAsync(s => s.Id == newStatusId);
            if (newStatus == null)
            {
                throw new Exception($"Status o ID '{newStatusId}' nie istnieje.");
            }
            
            if (!IsStatusTransitionAllowed(ticket.StatusId, newStatusId))
            {
                throw new Exception($"Przejście z statusu o ID '{ticket.StatusId}' do statusu o ID '{newStatusId}' nie jest dozwolone.");
            }

            ticket.StatusId = newStatusId;

            var nowUtc = DateTime.UtcNow;
            var wasPaused = ticket.PausedAtUtc.HasValue;
            var isPausedStatus = IsPausedStatus(newStatus.Name);

            if (wasPaused && !isPausedStatus)
            {
                var pausedDuration = nowUtc - ticket.PausedAtUtc!.Value;
                var pausedMinutes = (int)Math.Max(0, Math.Floor(pausedDuration.TotalMinutes));
                ticket.TotalPausedMinutes += pausedMinutes;
                ticket.PausedAtUtc = null;
            }
            else if (!wasPaused && isPausedStatus)
            {
                ticket.PausedAtUtc = nowUtc;
            }

            var isClosingStatus = IsClosedStatus(newStatus.Name);
            if (isClosingStatus && ticket.ResolvedAtUtc == null)
            {
                ticket.ResolvedAtUtc = nowUtc;
            }
            else if (!isClosingStatus)
            {
                ticket.ResolvedAtUtc = null;
            }

            await _context.SaveChangesAsync();

            await NotifyUsersAboutTicketAsync(ticket, "Zmiana statusu", $"Zmieniono status zgłoszenia: {ticket.Title} na '{newStatus.Name}'", currentUserId);
        }

        public async Task UpdatePriorityAsync(int ticketId, int newPriorityId)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
            if (ticket == null)
                throw new Exception($"Nie znaleziono zgłoszenia o ID: {ticketId}");

            var exists = await _context.Priorities.AnyAsync(p => p.Id == newPriorityId);
            if (!exists)
                throw new Exception($"Priorytet o ID '{newPriorityId}' nie istnieje.");

            ticket.PriorityId = newPriorityId;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(int ticketId, int newCategoryId)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
            if (ticket == null)
                throw new Exception($"Nie znaleziono zgłoszenia o ID: {ticketId}");

            var exists = await _context.Categories.AnyAsync(c => c.Id == newCategoryId);
            if (!exists)
                throw new Exception($"Kategoria o ID '{newCategoryId}' nie istnieje.");

            ticket.CategoryId = newCategoryId;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQueueAsync(int ticketId, int newQueueId)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
            if (ticket == null)
                throw new Exception($"Nie znaleziono zgłoszenia o ID: {ticketId}");

            var exists = await _context.Ques.AnyAsync(q => q.Id == newQueueId);
            if (!exists)
                throw new Exception($"Kolejka o ID '{newQueueId}' nie istnieje.");

            ticket.QueueId = newQueueId;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Status>> GetAllStatusesAsync()
        {
            return await _context.Statuses.ToListAsync();
        }

        public async Task UpdateClientAsync(int ticketId, int? newClientId)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
            if (ticket == null)
                throw new Exception($"Nie znaleziono zgłoszenia o ID: {ticketId}");

            if (newClientId.HasValue)
            {
                var exists = await _context.Clients.AnyAsync(c => c.Id == newClientId.Value);
                if (!exists)
                    throw new Exception($"Klient o ID '{newClientId.Value}' nie istnieje.");
            }

            ticket.ClientId = newClientId;
            await _context.SaveChangesAsync();
        }
        private bool IsStatusTransitionAllowed(int oldStatusId, int newStatusId)
        {
            var allowedTransitions = new Dictionary<int, List<int>>
            {
                { 1, new List<int> { 2, 4, 5 } },
                { 2, new List<int> { 1, 3, 4, 5, 6 } },
                { 3, new List<int> { 2 } },
                { 4, new List<int> { 2 } },
                { 5, new List<int> { 2 } },
                { 6, new List<int> { 2, 3 } }
            };

            return allowedTransitions.ContainsKey(oldStatusId) && 
                allowedTransitions[oldStatusId].Contains(newStatusId);
        }

        private async Task NotifyUsersAboutTicketAsync(Ticket ticket, string title, string message, int currentUserId)
        {
            var usersToNotify = await _context.Users
                .Include(u => u.Roles)
                .Include(u => u.Ques)
                .Include(u => u.AssignedTickets)
                .Where(u => u.Id != currentUserId && (
                    u.Roles.Any(r => r.Name == "Admin" || r.Name == "Helpdesk") ||
                    (u.Roles.Any(r => r.Name == "Technik") && 
                        (u.AssignedTickets.Any(t => t.Id == ticket.Id) || u.Ques.Any(q => q.Id == ticket.QueueId)))
                ))
                .ToListAsync();

            var notifications = usersToNotify.Select(u => new Notification
            {
                UserId = u.Id,
                Title = title,
                Message = message,
                TicketPublicId = ticket.PublicId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });

            _context.Notifications.AddRange(notifications);
            await _context.SaveChangesAsync();
        }
    }
}