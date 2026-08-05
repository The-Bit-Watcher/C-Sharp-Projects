using Microsoft.EntityFrameworkCore;
using HAS01.API.Models;

namespace HAS01.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public AppDbContext() { }

        public DbSet<Events> Events { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=INF354;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Events>().HasData(
                new Events { Id = 1, EventName = "Tech Workshop", Location = "Online", TicketPrice = (decimal)50.00f },
                new Events { Id = 2, EventName = "Music Festival", Location = "Cape Town", TicketPrice = (decimal)150.00f },
                new Events { Id = 3, EventName = "Art Exhibition", Location = "Johannesburg", TicketPrice = (decimal)75.00f },
                new Events { Id = 4, EventName = "Startup Summit", Location = "Durban", TicketPrice = (decimal)100.00f }
            );
        }
    }
}