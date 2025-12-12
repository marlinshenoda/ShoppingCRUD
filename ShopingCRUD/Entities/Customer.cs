using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopingCRUD.Entities
{
    public class Customer
    {
        //PK
        public int CustomerId { get; set; }
        [Required, MaxLength(100)]
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; } = "";

        [Required]
        public string Email { get; set; }
        public string City { get; set; }

        public List<Order> Orders= new List<Order>();


    }
}
