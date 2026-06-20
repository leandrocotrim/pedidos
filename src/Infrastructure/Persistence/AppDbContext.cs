using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(order =>
        {
            order.ToTable("orders");
            order.HasKey(o => o.Id);
            order.Property(o => o.Id).HasColumnName("id");
            order.Property(o => o.Type)
                 .HasColumnName("type")
                 .HasConversion<string>()
                 .HasMaxLength(20);
            order.Property(o => o.CreatedAt).HasColumnName("created_at");
            order.Property(o => o.Subtotal).HasColumnName("subtotal").HasPrecision(10, 2);
            order.Property(o => o.DiscountOrSurcharge).HasColumnName("discount_or_surcharge").HasPrecision(10, 2);
            order.Property(o => o.Total).HasColumnName("total").HasPrecision(10, 2);

            order.HasMany(o => o.Items)
                 .WithOne()
                 .HasForeignKey("order_id")
                 .OnDelete(DeleteBehavior.Cascade);

            order.Navigation(o => o.Items).HasField("_items");
        });

        modelBuilder.Entity<OrderItem>(item =>
        {
            item.ToTable("order_items");
            item.HasKey(i => i.Id);
            item.Property(i => i.Id).HasColumnName("id");
            item.Property(i => i.ProductId).HasColumnName("product_id");
            item.Property(i => i.ProductName).HasColumnName("product_name").HasMaxLength(255);
            item.Property(i => i.Quantity).HasColumnName("quantity");
            item.Property(i => i.UnitPrice).HasColumnName("unit_price").HasPrecision(10, 2);
            item.Property(i => i.Subtotal).HasColumnName("subtotal").HasPrecision(10, 2);
        });

        modelBuilder.Entity<Product>(product =>
        {
            product.ToTable("products");
            product.HasKey(p => p.Id);
            product.Property(p => p.Id).HasColumnName("id");
            product.Property(p => p.Name).HasColumnName("name").HasMaxLength(255);
            product.Property(p => p.UnitPrice).HasColumnName("unit_price").HasPrecision(10, 2);
            product.Property(p => p.CreatedAt).HasColumnName("created_at");
        });
    }
}
