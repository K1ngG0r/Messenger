using Microsoft.EntityFrameworkCore;

namespace Server
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Data/Database/messenger.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                
                entity.Property(u => u.UserName)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(u => u.LocalServerAddress)
                    .IsRequired()
                    .HasMaxLength(45);
                
                entity.Property(u => u.LocalServerPort)
                    .IsRequired();
                
                // Игнорируем вычисляемое свойство
                entity.Ignore(u => u.LocalServer);
                
                // Связь один-ко-многим с Message
                entity.HasMany(u => u.UnreadMessages)
                    .WithOne(m => m.Sender)
                    .HasForeignKey(m => m.Id)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Настройка Message
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(m => m.Id);
                
                entity.Property(m => m.Id)
                    .ValueGeneratedOnAdd();
                
                entity.Property(m => m.MessageText)
                    .IsRequired()
                    .HasMaxLength(1000);
                
                // Индексы для быстрого поиска
                entity.HasIndex(m => m.ChatId);
                entity.HasIndex(m => m.UserSessionKey);
            });
        }
    }
}