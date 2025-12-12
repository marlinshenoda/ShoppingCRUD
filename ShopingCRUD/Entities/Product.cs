using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShopingCRUD.Entities
{
    public class Product 
    {
        public int ProductId { get; set; }

        [Required, MaxLength(100)]
        public string ProductName { get; set; }

        public decimal ProductPrice { get; set; }
        public string ProductDescription { get; set; }

        public List<OrderRow> OrderRows = new List<OrderRow>();

    }
}
