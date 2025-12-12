using Microsoft.EntityFrameworkCore;
using ShopingCRUD.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopingCRUD.Data
{
    /// <summary>
    /// Provides initial seed data for the database.
    /// Call `SeedData.Initialize(context)` during app startup to ensure
    /// the database is migrated and populated with initial records.
    /// </summary>
    public static class SeedData
    {
        /// <summary>
        /// Initialize the database: print DB path, apply migrations and add seed data
        /// for customers, products and an example order if none exist.
        /// </summary>
        /// <param name="Context">An instance of <see cref="ShopDbContext"/> to use for seeding.</param>
        public static async Task Initialize(ShopDbContext Context)
        {
            // Print the expected database file path (helpful when using SQLite or
            // diagnosing where the DB file is located during development).
            Console.WriteLine("DB:" + Path.Combine(AppContext.BaseDirectory, "shop.db"));

            // NOTE: This creates a new context instance but the variable `db` is never used.
            // Either remove this line or use the created context instead of the passed-in `Context`.
            await using var db = new ShopDbContext();

            // Apply any pending EF Core migrations.
            // Consider using the async API `await Context.Database.MigrateAsync()` in async methods.
            Context.Database.Migrate();

            // --- SEED CUSTOMERS ---
            // Only add customers if the table is empty to avoid duplicate seed data on multiple runs.
            if (!Context.Customers.Any())
            {
                Context.Customers.AddRange(
                     new Customer { CustomerName = "Anna Svensson", PhoneNumber="0745252222" ,Email = "anna@mail.com", City = "Stockholm" },
                     new Customer { CustomerName = "Erik Karlsson", PhoneNumber = "0745252222", Email = "erik@mail.com", City = "Göteborg" }
                    );

                // Persist changes. In an async method prefer `await Context.SaveChangesAsync()`.
                Context.SaveChanges();
            }

            // --- SEED PRODUCTS ---
            // Same pattern: only seed products if none exist.
            if (!Context.Products.Any())
            {
                Context.Products.AddRange(
                    new Product { ProductName = "Laptop", ProductPrice = 9999, ProductDescription = "Gaming laptop" },
                    new Product { ProductName = "USB Cable", ProductPrice = 199,ProductDescription= "USB Cable" },
                    new Product { ProductName = "Headphones", ProductPrice = 499,ProductDescription="white" }
                );

                Context.SaveChanges();
            }

            // --- SEED ORDERS ONLY IF NONE EXISTS ---
            // Create an example order that references previously seeded customer and product.
            if (!Context.Orders.Any())
            {
                // Resolve seeded entities by unique fields (email, product name).
                var anna = Context.Customers.First(c => c.Email == "anna@mail.com");
                var laptop = Context.Products.First(p => p.ProductName == "Laptop");

                var order = new Order
                {
                    CustomerId = anna.CustomerId,
                    OrderDate = DateTime.Now.AddDays(-2),
                    Status = "Paid",
                    TotalAmount = laptop.ProductPrice,

                    // Using a collection initializer for `OrderRows`. This requires that the
                    // `OrderRows` collection on the `Order` instance is already initialized
                    // (e.g. to a new List<OrderRow>() in the entity definition). If it's null
                    // this initializer will throw a NullReferenceException.
                    OrderRows =
                    {
                        new OrderRow
                        {
                            ProductId = laptop.ProductId,
                            Quantity = 1,
                            UnitPrice = laptop.ProductPrice
                        }
                    }
                };

                Context.Orders.Add(order);
                Context.SaveChanges();
            }
        }
    }
}
