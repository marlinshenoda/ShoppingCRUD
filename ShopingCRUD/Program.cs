using Microsoft.EntityFrameworkCore;
using ShopingCRUD.Data;
using ShopingCRUD.Services;
using System.Threading.Channels;

Console.WriteLine("DB:" + Path.Combine(AppContext.BaseDirectory, "shop.db"));

var context = new ShopDbContext();
context.Database.MigrateAsync();
var customerService = new CustomerService(context);
var orderService = new OrderService(context);
var orderSummary= new OrderSummaryService(context);
var productService= new ProductServices(context);
// Seeding
await SeedData.Initialize(context);

while (true)
{
    Console.WriteLine("Products | addproduct |");
     Console.WriteLine("customers |filter-city| addcustomer | editcustomer | deletecustomer |");   
   Console.WriteLine("listordersummary |SummariesPage|orderspage| addorder |orders | orderspage");  
       
    var input = Console.ReadLine().Split(' ');

    switch (input[0])
    {
        case "products":
            productService.ListProductAsync();
            break;
        case "addproduct":
            productService.AddProductsAsync();
            break;
        case "listordersummary":
            orderSummary.ListOrdersummarySummaries();
            break;
        case "SummariesPage":
            orderSummary.SummariesPage(int.Parse(input[1]), int.Parse(input[2]));
            break;
        case "customers":
            customerService.ListCustomers();
            break;
        case "addcustomer":
            customerService.AddCustomer();
            break;

        case "editcustomer":
            customerService.EditCustomer(int.Parse(input[1]));
            break;

        case "deletecustomer":
            customerService.DeleteCustomer(int.Parse(input[1]));
            break;
        case "filter-city":
            customerService.FilterByCity();
            break;

        case "orders":
            orderService.ListOrderAsync();
            break;

        //case "orderdetails":
        //    orderService.OrderDetails(int.Parse(input[1]));
        //    break;

        case "addorder":
               orderService.AddOrderAsync();
               break;

            //case "ordersbystatus":
            //    orderService.OrdersByStatus(input[1]);
            //    break;

            //case "ordersbycustomer":
            //    orderService.OrdersByCustomer(int.Parse(input[1]));
            //    break;

            case "orderspage":
            Console.Write("Page: ");
            if (!int.TryParse(Console.ReadLine(), out var page))
                page = 1;

            Console.Write("PageSize: ");
            if (!int.TryParse(Console.ReadLine(), out var pageSize))
                pageSize = 10;

            orderService.ListOrdersPagedAsync(page, pageSize);
            break;




    }
}
