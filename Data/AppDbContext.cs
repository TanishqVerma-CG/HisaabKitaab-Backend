using Microsoft.EntityFrameworkCore;
using BillingSystem.Models;

namespace BillingSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Party> Parties { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<CustomerProductPrice> CustomerProductPrices { get; set; }
    public DbSet<Bill> Bills { get; set; }
    public DbSet<BillItem> BillItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Party>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.GstNumber).HasMaxLength(15);
            entity.Property(e => e.Phone).HasMaxLength(15);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.BasePrice).HasPrecision(18, 2);
            entity.Property(e => e.GstRate).HasPrecision(5, 2);
        });

        modelBuilder.Entity<CustomerProductPrice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasPrecision(18, 2);

            entity.HasOne(e => e.Customer)
                .WithMany(p => p.CustomerProductPrices)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Product)
                .WithMany(p => p.CustomerProductPrices)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.CustomerId, e.ProductId }).IsUnique();
        });

        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BillNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Subtotal).HasPrecision(18, 2);
            entity.Property(e => e.GstAmount).HasPrecision(18, 2);
            entity.Property(e => e.GrandTotal).HasPrecision(18, 2);

            entity.HasOne(e => e.Customer)
                .WithMany(p => p.Bills)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.BillNumber).IsUnique();
        });

        modelBuilder.Entity<BillItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantity).HasPrecision(18, 2);
            entity.Property(e => e.Rate).HasPrecision(18, 2);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.GstRate).HasPrecision(5, 2);

            entity.HasOne(e => e.Bill)
                .WithMany(b => b.BillItems)
                .HasForeignKey(e => e.BillId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Product)
                .WithMany(p => p.BillItems)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}