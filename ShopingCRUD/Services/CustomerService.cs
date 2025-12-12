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
    public class CustomerService
    {
        private readonly ShopDbContext _context;
        private readonly JsonStorageService<Customer> _customerJson;

        public CustomerService(ShopDbContext context)
        {
            _context = context;
            _customerJson = new JsonStorageService<Customer>("customers.json");

        }
        // LISTA KUNDER
        public  void ListCustomers()
        {
            //using var db= new ShopDbContext();

            var customers = _context.Customers.ToList();
            foreach(var c in customers) {
                Console.WriteLine($"{c.CustomerId}: {c.CustomerName} - {c.PhoneNumber} - {c.Email} - {c.City}");

            }
        }
        public async Task AddCustomer()
        {
            Console.Write("Name: ");
            var name = Console.ReadLine();

            Console.Write("Phone number: ");
            var PhoneNumber = Console.ReadLine()!;

            Console.Write("Email: ");
            var email = Console.ReadLine();

            Console.Write("City: ");
            var city = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Name cannot be empty.");
                return;
            }
            try
            {
                var cust = new Customer
                {
                    CustomerName = name,
                    PhoneNumber= PhoneNumber,
                    Email = email,
                    City = city
                    
                };

                // Spara i databas
                _context.Customers.Add(cust);
                _context.SaveChanges();

                // Spara i JSON
                await _customerJson.AddAsync(cust);

                Console.WriteLine("Customer added i Json and DataBase!");

            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Error: Email may already exist.");
            }
        }
        // REDIGERA KUND
        public void EditCustomer(int id ) { 
        var customer =   _context.Customers.Find(id);

            if (customer == null) {
                Console.WriteLine("Customer not found");
                return;
            }

            Console.Write($"Name ({customer.CustomerName}): ");
            var name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name)) customer.CustomerName = name;

            Console.Write($"Email ({customer.Email}): ");
            var email = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(email)) customer.Email = email;

            Console.Write($"City ({customer.City}): ");
            var city = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(city)) customer.City = city;

            _context.SaveChanges();
            Console.WriteLine("Customer updated.");

        }
        // TA BORT KUND
        public void DeleteCustomer(int id)
        {
            var customer = _context.Customers.Include(c => c.Orders).FirstOrDefault(c => c.CustomerId == id);

            if (customer == null)
            {
                Console.WriteLine("Customer not found.");
                return;
            }

            _context.Customers.Remove(customer);
            _context.SaveChanges();
            Console.WriteLine("Customer deleted.");
        }
        public IEnumerable<Customer> GetByCity(string city) 
            {
            return _context.Customers.Where(c => c.City == city);

            }
        public void FilterByCity()
        {
            Console.Write("Enter city: ");
            var city = Console.ReadLine()!;

            var customers = GetByCity(city);

            Console.WriteLine($"Customers in {city}:");

            foreach (var c in customers)
            {
                Console.WriteLine($"{c.CustomerId} | {c.CustomerName} | {c.Email} | {c.City} | {c.PhoneNumber}");
            }
        }

    }
}
