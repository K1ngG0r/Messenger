using Client.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Data
{
    public class AppDBContext : DbContext
    {
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatMessage> Messages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Participant> Participants { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=XAMMessenger;Trusted_Connection=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ChatMessage>().HasOne(x => x.Chat).WithMany(x => x.Messages);
            modelBuilder.Entity<PrivateChat>().ToTable("PrivateChats");
            modelBuilder.Entity<GroupChat>().ToTable("GroupChats");
            modelBuilder.Entity<ChannelChat>().ToTable("ChannelChats");

            modelBuilder.Entity<ChannelChat>()
                .HasOne<Chat>()
                .WithOne()
                .HasForeignKey<ChannelChat>(c => c.Id)
                .HasPrincipalKey<Chat>(c => c.Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GroupChat>()
                .HasOne<Chat>()
                .WithOne()
                .HasForeignKey<GroupChat>(c => c.Id)
                .HasPrincipalKey<Chat>(c => c.Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PrivateChat>()
                .HasOne<Chat>()
                .WithOne()
                .HasForeignKey<PrivateChat>(c => c.Id)
                .HasPrincipalKey<Chat>(c => c.Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Chat>()
                .HasMany(c => c.Messages)
                .WithOne(m => m.Chat)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Participant>()
                .HasOne(p => p.Chat)
                .WithMany() 
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Participant>()
                .HasOne(p => p.User)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);


            /*modelBuilder.Entity<GroupChat>()
                .HasOne(gc => gc.Owner)
                .WithMany()
                .HasForeignKey("OwnerId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<ChannelChat>()
                .HasOne(cc => cc.Owner)
                .WithMany()
                .HasForeignKey("OwnerId") 
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();*/

            modelBuilder.Entity<PrivateChat>()
                .HasOne(pc => pc.Correspondent)
                .WithMany()
                .HasForeignKey("CorrespondentUserId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }
    }
}
