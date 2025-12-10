using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace BeverageBackend.Models
{
    public class BeverageDbContext:DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public BeverageDbContext(DbContextOptions<BeverageDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(ci => new { ci.CartId, ci.ProductId });
                entity.HasOne(c => c.Cart).WithMany(ci => ci.CartItems).HasForeignKey(c => c.CartId);
                entity.HasOne(p => p.Product).WithMany(ci => ci.CartItems).HasForeignKey(p => p.ProductId);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => new { oi.OrderId, oi.ProductId });
                entity.HasOne(o => o.Order).WithMany(oi => oi.OrderItems).HasForeignKey(o => o.OrderId);
                entity.HasOne(p => p.Product).WithMany(oi => oi.OrderItems).HasForeignKey(p => p.ProductId);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(cus => cus.Id);
                entity.HasOne(cus => cus.Cart).WithOne(ca => ca.Customer).HasForeignKey<Cart>(ca => ca.CustomerId);
            });
        }
    }
}
