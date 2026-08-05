using Ass3WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Ass3WebApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<WatchlistItem> WatchlistItems { get; set; }
        public DbSet<WatchedItem> WatchedItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Movie>()
                .HasIndex(m => m.ImdbId)
                .IsUnique();

            modelBuilder.Entity<WatchlistItem>()
                .HasIndex(w => new { w.UserId, w.MovieId })
                .IsUnique();

            modelBuilder.Entity<WatchedItem>()
                .HasIndex(w => new { w.UserId, w.MovieId })
                .IsUnique();
        }
    }
}