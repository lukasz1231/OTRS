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

            if (!context.Statuses.Any(s => s.Name == "Wykonane"))
            {
                context.Statuses.Add(new Status { Name = "Wykonane", Description = "Zadanie zrealizowane przez technika" });
                context.SaveChanges();
            }

            if (!context.Users.Any())
            {
                var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");
                var technikRole = context.Roles.FirstOrDefault(r => r.Name == "Technik");
                var klientRole = context.Roles.FirstOrDefault(r => r.Name == "Klient");

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
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("useruser")
                };
                if (klientRole != null) standardUser.Roles.Add(klientRole);

                context.Users.AddRange(adminUser, technikUser, standardUser);
                context.SaveChanges();
            }
        }
    }
}