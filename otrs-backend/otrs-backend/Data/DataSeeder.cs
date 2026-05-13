using otrs_backend.Models;

namespace otrs_backend.Data
{
    public static class DataSeeder
    {
        public static void SeedRoles(AppDbContext context)
        {
            if (!context.Roles.Any())
            {
                var roles = new List<Role>
                {
                    new Role { Name = "Admin", Description = "Administrator systemu" },
                    new Role { Name = "Helpdesk", Description = "I linia wsparcia" },
                    new Role { Name = "Technik", Description = "II linia wsparcia" },
                    new Role { Name = "Klient", Description = "Użytkownik końcowy" }
                };

                context.Roles.AddRange(roles);
                context.SaveChanges();
            }
        }

        public static void SeedStatuses(AppDbContext context)
        {
            if (!context.Statuses.Any(s => s.Name == "Wykonane"))
            {
                context.Statuses.Add(new Status { Name = "Wykonane", Description = "Zadanie zrealizowane przez technika" });
                context.SaveChanges();
            }
        }

        public static void SeedClients(AppDbContext context)
        {
            if (!context.Clients.Any())
            {
                var client = new Client
                {
                    Name = "Hustletrack ITSM",
                    Description = "Główny klient systemowy"
                };
                context.Clients.Add(client);
                context.SaveChanges();
            }
        }

        public static void SeedUsers(AppDbContext context)
        {
            if (!context.Users.Any())
            {
                var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");
                var technikRole = context.Roles.FirstOrDefault(r => r.Name == "Technik");
                var klientRole = context.Roles.FirstOrDefault(r => r.Name == "Klient");
                
                var defaultClient = context.Clients.FirstOrDefault(c => c.Name == "Hustletrack ITSM");

                var adminUser = new User
                {
                    Name = "Admin",
                    Surname = "Systemowy",
                    Email = "admin@wp.pl",
                    Bio = "Główny administrator systemu",
                    AvatarUrl = "",
                    BirthDate = DateTime.Now,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("adminadmin")
                };
                if (adminRole != null) adminUser.Roles.Add(adminRole);

                var technikUser = new User
                {
                    Name = "Technik",
                    Surname = "Wsparcia",
                    Email = "technik@wp.pl",
                    Bio = "Pracownik techniczny",
                    AvatarUrl = "",
                    BirthDate = DateTime.Now,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("techniktechnik")
                };
                if (technikRole != null) technikUser.Roles.Add(technikRole);

                var standardUser = new User
                {
                    Name = "Jan",
                    Surname = "Kowalski",
                    Email = "user@wp.pl",
                    Bio = "Zwykły użytkownik",
                    AvatarUrl = "",
                    BirthDate = DateTime.Now,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("useruser"),
                    ClientId = defaultClient?.Id
                };
                if (klientRole != null) standardUser.Roles.Add(klientRole);

                context.Users.AddRange(adminUser, technikUser, standardUser);
                context.SaveChanges();
            }
        }

        public static void SeedTickets(AppDbContext context)
        {
            if (!context.Tickets.Any())
            {
                var defaultClient = context.Clients.FirstOrDefault(c => c.Name == "Hustletrack ITSM");

                // Ensure required dictionary data exists before creating a ticket
                var category = context.Categories.FirstOrDefault() ?? new Category { Name = "Sprzęt", Description = "Awarie sprzętowe", ClientId = defaultClient?.Id };
                var priority = context.Priorities.FirstOrDefault() ?? new Priority { Name = "Wysoki", Description = "Wysoki priorytet", Level = 1 };
                var type = context.Types.FirstOrDefault() ?? new Models.Type { Name = "Incydent", Description = "Zgłoszenie awarii" };
                var queue = context.Ques.FirstOrDefault() ?? new Que { Name = "IT Support" };
                var status = context.Statuses.FirstOrDefault(s => s.Name == "Nowy") ?? new Status { Name = "Nowy", Description = "Nowe zgłoszenie" };

                if (category.Id == 0) context.Categories.Add(category);
                if (priority.Id == 0) context.Priorities.Add(priority);
                if (type.Id == 0) context.Types.Add(type);
                if (queue.Id == 0) context.Ques.Add(queue);
                if (status.Id == 0) context.Statuses.Add(status);
                
                context.SaveChanges();

                var clientUser = context.Users.FirstOrDefault(u => u.Email == "user@wp.pl");
                
                if (clientUser != null)
                {
                    var ticket = new Ticket
                    {
                        PublicId = "PL" + DateTime.Now.ToString("yyyyMMdd") + "00001",
                        Title = "Testowe zgłoszenie - Awaria komputera",
                        Description = "Mój komputer nie chce się włączyć. Proszę o pilną pomoc.",
                        CreatedAt = DateTime.UtcNow,
                        CreatorId = clientUser.Id,
                        CategoryId = category.Id,
                        PriorityId = priority.Id,
                        TypeId = type.Id,
                        QueueId = queue.Id,
                        StatusId = status.Id
                    };
                    
                    context.Tickets.Add(ticket);
                    context.SaveChanges();
                }
            }
        }
    }
}