using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Server
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Chat> Chats { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=messenger.db");
                //optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Конфигурация User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                
                entity.Property(u => u.UserName)
                    .IsRequired()
                    .HasMaxLength(100);
                    
                entity.HasIndex(u => u.UserName)
                    .IsUnique();
            });

            // Конфигурация Message
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(m => m.Id);
                
                entity.Property(m => m.MessageText)
                    .IsRequired()
                    .HasMaxLength(1000);
                entity.HasIndex(m => m.ChatId);
            });
        }
    }
}
