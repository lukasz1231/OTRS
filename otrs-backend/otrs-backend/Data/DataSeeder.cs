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
    }
}