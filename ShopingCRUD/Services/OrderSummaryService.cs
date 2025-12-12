using Microsoft.EntityFrameworkCore;
using ShopingCRUD.Data;
using ShopingCRUD.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopingCRUD.Services
{
    public class OrderSummaryService
    {
        private readonly ShopDbContext _context;

        public OrderSummaryService(ShopDbContext context)
        {
            _context = context;
        }
        // Hämta ALLA order-sammanfattningar
        public async Task ListOrdersummarySummaries()
        {
            var summaries = await _context.OrderSummaries
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            foreach (var s in summaries)
            {
                Console.WriteLine($"{s.OrderId} | {s.OrderDate:d} | {s.CustomerName} | {s.CustomerEmail} | {s.TotalAmount} kr");
            }
        }

        

        // Hämta en specifik order-sammanfattning
        public void GetSummary(int orderId)
        {
            var summary = _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.OrderId == orderId)
                .Select(o => new OrderSummary
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    CustomerName = o.Customer.CustomerName,
                    CustomerEmail = o.Customer.Email,
                    TotalAmount = o.TotalAmount
                })
                .FirstOrDefault();

            if (summary == null)
            {
                Console.WriteLine("Order not found.");
                return;
            }

            Console.WriteLine($"\nOrder Summary #{summary.OrderId}");
            Console.WriteLine($"Date: {summary.OrderDate}");
            Console.WriteLine($"Customer: {summary.CustomerName}");
            Console.WriteLine($"Email: {summary.CustomerEmail}");
            Console.WriteLine($"Total Amount: {summary.TotalAmount} kr");
        }

        // Filtrera på kund
        public void SummariesByCustomer(int customerId)
        {
            var result = _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.CustomerId == customerId)
                .Select(o => new OrderSummary
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    CustomerName = o.Customer.CustomerName,
                    CustomerEmail = o.Customer.Email,
                    TotalAmount = o.TotalAmount
                })
                .ToList();

            foreach (var s in result)
            {
                Console.WriteLine(
                    $"{s.OrderId} | {s.OrderDate:d} | {s.CustomerName} | {s.CustomerEmail} | {s.TotalAmount} kr");
            }
        }

        // Paging (Skip & Take)
        public void SummariesPage(int page, int pageSize)
        {
            var result = _context.Orders
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderSummary
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    CustomerName = o.Customer.CustomerName,
                    CustomerEmail = o.Customer.Email,
                    TotalAmount = o.TotalAmount
                })
                .ToList();

            foreach (var s in result)
            {
                Console.WriteLine(
                    $"{s.OrderId} | {s.OrderDate} | {s.CustomerName} | {s.TotalAmount} kr");
            }
        }

    }
}
