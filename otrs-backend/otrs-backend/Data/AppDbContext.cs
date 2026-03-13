using Microsoft.EntityFrameworkCore;
using otrs_backend.Models;
using System.Net.Mail;

namespace otrs_backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Status> Statuses => Set<Status>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Priority> Priorities => Set<Priority>();
        public DbSet<Que> Ques => Set<Que>();
        public DbSet<Models.Type> Types => Set<Models.Type>();
        public DbSet<otrs_backend.Models.Attachment> Attachments { get; set; }
        public DbSet<Client> Clients => Set<Client>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User ↔ Role
            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity(j => j.ToTable("UserRoles"));

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            // Ticket → Creator
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Creator)
                .WithMany(u => u.CreatedTickets)
                .HasForeignKey(t => t.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ticket → Type 
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Type)
                .WithMany()
                .HasForeignKey(t => t.TypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ticket → Queue 
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Queue)
                .WithMany()
                .HasForeignKey(t => t.QueueId)
                .OnDelete(DeleteBehavior.Restrict);
            // Ticket ↔ AssignedUsers
            modelBuilder.Entity<Ticket>()
                .HasMany(t => t.AssignedUsers)
                .WithMany(u => u.AssignedTickets)
                .UsingEntity(j => j.ToTable("TicketAssignments"));

            // Ticket → Category
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Category)
                .WithMany()
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ticket → Priority
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Priority)
                .WithMany()
                .HasForeignKey(t => t.PriorityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ticket → Status
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Status)
                .WithMany()
                .HasForeignKey(t => t.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Comment → Ticket
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Ticket)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // Comment → User
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // User ↔ Que (Wiele-do-Wielu)
            // Ta konfiguracja pozwala jednemu użytkownikowi należeć do wielu kolejek jednocześnie
            // Relacja Wiele-do-Wielu: Użytkownicy <-> Kolejki
            modelBuilder.Entity<User>()
                .HasMany(u => u.Ques)
                .WithMany(q => q.Users)
                .UsingEntity(j => j.ToTable("UserQueues")); // Ta tabela zostanie stworzona w SQLite

            // Attachment → Comment
            modelBuilder.Entity<otrs_backend.Models.Attachment>()
                .HasOne(a => a.Comment)
                .WithMany(c => c.Attachments)
                .HasForeignKey(a => a.CommentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
