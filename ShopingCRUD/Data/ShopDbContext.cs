using Microsoft.EntityFrameworkCore;
using ShopingCRUD.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopingCRUD.Data
{
    public class ShopDbContext: DbContext
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderRow > OrderRows => Set<OrderRow>();
       public DbSet<OrderSummary> OrderSummaries => Set<OrderSummary>();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(AppContext.BaseDirectory, "Shop.db");
            optionsBuilder.UseSqlite(connectionString: $"Filename={dbPath}");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderSummary>(e =>
            {
                e.HasNoKey();
                e.ToView("OrderSummaryView");
            });
            modelBuilder.Entity<Product>(e =>
            {
                e.HasKey(e => e.ProductId);
                e.Property(e => e.ProductName).IsRequired().HasMaxLength(100);
                e.Property(e => e.ProductDescription);
                e.Property(e => e.ProductPrice);
                
             

            });
            modelBuilder.Entity<Customer>(e =>
            {
                e.HasKey(e => e.CustomerId);
                e.Property(e => e.CustomerName).IsRequired().HasMaxLength(100);
                e.Property(e => e.Email).IsRequired();
                e.Property(e => e.City);

                e.HasIndex(e => e.Email).IsUnique();

            });
            modelBuilder.Entity<Order>(e =>
            {
                e.HasKey(e => e.OrderId);
                e.Property(e => e.Status);
                e.Property(e => e.TotalAmount).IsRequired();
              
                e.HasOne(e => e.Customer).WithMany(e=>e.Orders).HasForeignKey(e=> e.CustomerId).OnDelete(DeleteBehavior.Cascade);

            });
            modelBuilder.Entity<OrderRow>(e =>
            {
                e.HasKey(e => new { e.OrderRowId});

                e.Property(e => e.Quantity);
                e.Property(e => e.UnitPrice).IsRequired();
                e.HasOne(e => e.Product).WithMany(e => e.OrderRows).HasForeignKey(e=>e.ProductId).OnDelete(DeleteBehavior.Restrict);// Vanligt: hindra radering av produkt om den finns i orderrad
                e.HasOne(e => e.Order).WithMany(e => e.OrderRows).HasForeignKey(e => e.OrderId).OnDelete(DeleteBehavior.Cascade); // T.ex. ta bort rad när order tas bort

            });
        }
    }
}
