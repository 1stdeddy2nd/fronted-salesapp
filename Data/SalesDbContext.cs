using Microsoft.EntityFrameworkCore;
using SalesAnalytics.Dto;
using SalesAnalytics.Models;

namespace SalesAnalytics.Data
{
    public class SalesDbContext : DbContext
    {
        public SalesDbContext(DbContextOptions<SalesDbContext> options)
            : base(options) { }

        public DbSet<Sales> Sales { get; set; }
        public DbSet<SalesSummaryDto> SalesSummaries { get; set; }

        public DbSet<SalesProduct> DimProducts { get; set; }
        public DbSet<SalesRegion> DimRegions { get; set; }
        public DbSet<SalesCustomer> DimCustomers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sales>(entity =>
            {
                entity.ToTable("Sales");

                entity.HasKey(s => s.OrderId);

                entity.Property(s => s.OrderId).HasColumnName("order_id");
                entity.Property(s => s.Customer).HasColumnName("customer_id");
                entity.Property(s => s.Product).HasColumnName("product");
                entity.Property(s => s.Region).HasColumnName("region");
                entity.Property(s => s.Amount).HasColumnName("amount").HasPrecision(10, 2);
                entity.Property(s => s.Timestamp).HasColumnName("timestamp");
                entity.Property(s => s.CreatedAt).HasColumnName("created_at");
                entity.Property(s => s.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<SalesSummaryDto>().HasNoKey(); // Required for stored procedure result
        }
    }
}
