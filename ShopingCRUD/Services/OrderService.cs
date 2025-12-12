using Microsoft.EntityFrameworkCore;
using ShopingCRUD.Data;
using ShopingCRUD.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ShopingCRUD.Services
{
    public class OrderService
    {
        private readonly ShopDbContext _context;

        public OrderService(ShopDbContext context)
        {
            _context = context;
        }
        public async void ListOrderAsync()
        {
            var Orders = await _context.Orders.AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.OrderRows)
                .OrderByDescending(o => o.OrderId)

                 .ToListAsync();

            foreach (var order in Orders)
            {
                Console.WriteLine(
             $"{order.OrderId} | {order.Customer?.CustomerName} | {order.OrderDate} | {order.TotalAmount} | {order.Status}");
            }


        }

        public async void AddOrderAsync()
        {
            var customers = await _context.Customers.AsNoTracking().OrderBy(a => a.CustomerId).ToListAsync();
            foreach (var customer in customers)
            {
                Console.WriteLine($"{customer.CustomerId} | {customer.CustomerName} | {customer.Email}");
            }
            Console.WriteLine("write customer id");
            if (!int.TryParse(Console.ReadLine(), out var CustomerId) || (!customers.Any(a => a.CustomerId == CustomerId)))

            {
                Console.WriteLine("invalid input");
                return;
            }
            var order = new Order
            {
                CustomerId = CustomerId,
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalAmount = 0,

            };
            var OrderRows = new List<OrderRow>();
            while (true)
            {
                Console.WriteLine("Add order row? Y/N");
                var answer = Console.ReadLine().Trim().ToLowerInvariant();
                if (answer != "y") break;


                var products = await _context.Products.AsNoTracking().OrderBy(a => a.ProductId).ToListAsync();
                foreach (var p in products)
                {
                    Console.WriteLine($"{p.ProductId} | {p.ProductName} | {p.ProductPrice}");
                }
                Console.WriteLine("write product id");
                if (!int.TryParse(Console.ReadLine(), out var productId) || (!products.Any(a => a.ProductId == productId)))

                {
                    Console.WriteLine("invalid input");
                    return;
                }
                var chosenProduct = await _context.Products.FirstOrDefaultAsync(a => a.ProductId == productId);
                if (chosenProduct == null)
                {
                    Console.WriteLine("Product not found");
                    continue;

                }
                Console.WriteLine("quantity:");
                if (!int.TryParse(Console.ReadLine(), out var quantity) || (quantity <= 0))
                {
                    Console.WriteLine("invalid input of quantity");
                    continue;

                }
                var row = new OrderRow
                {
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = chosenProduct.ProductPrice
                };
                order.OrderRows.Add(row);
            }


            // 4. Beräkna totalbelopp
            order.TotalAmount = order.OrderRows.Sum(r => r.Quantity * r.UnitPrice);

            // 5. Spara ordern
            _context.Orders.Add(order);
            _context.OrderRows.AddRange(order.OrderRows);

            await _context.SaveChangesAsync();

            Console.WriteLine($"Order skapad! OrderId = {order.OrderId}, Totalbelopp = {order.TotalAmount}");
        }
        public void ListOrdersPagedAsync(int page, int pageSize)
        {
           

            var query = _context.Orders
                .Include(x => x.Customer)
                .AsNoTracking()
                .OrderByDescending(x => x.OrderDate);

            var totalCount =  query.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var orders =  query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)

                .ToList();
            Console.WriteLine($"Page {page} / {totalPages}, pageSize = {pageSize}");
            foreach (var order in orders)
            {
                Console.WriteLine(
                    $"{order.OrderId} | {order.OrderDate} | {order.TotalAmount:c} | {order.Customer?.Email}");
            }
        }
        public void OrdersPage(int page, int pageSize)
        {
            var orders = _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(o => o.Customer)
                .ToList();

            foreach (var o in orders)
                Console.WriteLine($"{o.OrderId} - {o.Customer.CustomerName} - {o.OrderDate} - {o.TotalAmount} kr");
        }

    }
}
