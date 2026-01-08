using Microsoft.EntityFrameworkCore;
using Template.Models;

namespace Template.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) 
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Crop> Crops { get; set; }
        public DbSet<MarketPrice> MarketPrices { get; set; }
        public DbSet<SoilReport> SoilReports { get; set; }
        public DbSet<TrainingRequest> TrainingRequests { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Fertilizer> Fertilizers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique Email constraint
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Decimal precision for prices
            modelBuilder.Entity<MarketPrice>()
                .Property(m => m.Price)
                .HasPrecision(18, 2);

            // Decimal precision for pH
            modelBuilder.Entity<SoilReport>()
                .Property(s => s.pH)
                .HasPrecision(4, 2);
        }
    }
}
