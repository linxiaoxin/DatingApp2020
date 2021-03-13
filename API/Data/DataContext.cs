using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder){
            base.OnModelCreating(modelBuilder);    

            modelBuilder.Entity<UserLikes>()
                .HasKey(k => new{k.likedUserId, k.likedByUserId});

            modelBuilder.Entity<UserLikes>()
                .HasOne(x => x.likedByUser)
                .WithMany(l => l.LikedUsers)
                .HasForeignKey(k => k.likedByUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserLikes>()
                .HasOne(x => x.likedUser)
                .WithMany(l => l.LikedByUsers)
                .HasForeignKey(k => k.likedUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Recipient)
                .WithMany(u => u.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);
        }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserLikes> Likes {get; set;}
        public DbSet<Message> Messages { get; set; }
    }
}