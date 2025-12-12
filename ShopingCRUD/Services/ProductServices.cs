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
    public class ProductServices
    {
        private readonly ShopDbContext _context;

        public ProductServices(ShopDbContext context)
        {
            _context = context;
        }
        public async void ListProductAsync()
        {
            var products= await _context.Products.AsNoTracking().OrderBy(p=>p.ProductId).ToListAsync();
            foreach (var p in products) {
                Console.WriteLine( $"{p.ProductId} | {p.ProductName}| {p.ProductPrice} | {p.ProductDescription}");
            }
        }
        public async void AddProductsAsync()
        {
            Console.WriteLine("write product name:");
            var productName=Console.ReadLine();
            Console.WriteLine("write product Price:");
            var productPrice=decimal.Parse(Console.ReadLine());
            Console.WriteLine("write product description:");
            var productdesc=Console.ReadLine();
            var product = new Product
            {
                ProductName=productName,
                ProductPrice=productPrice,
                ProductDescription=productdesc

            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            Console.WriteLine("Product added successfuly!");

        }
    }
}
