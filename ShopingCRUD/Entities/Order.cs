using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShopingCRUD.Entities
{
    public class Order
    {
        public int OrderId {  get; set; }

        [Required]
        public DateTime OrderDate { get; set; }
        public string Status{ get; set; }

        [Required]
        public decimal TotalAmount {  get; set; }

        public int CustomerId {  get; set; }
        public Customer? Customer { get; set; }
       public  List<OrderRow> OrderRows { get; set; } = new List<OrderRow>();

    }
}
