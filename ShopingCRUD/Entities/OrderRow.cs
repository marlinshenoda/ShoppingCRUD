using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopingCRUD.Entities
{
    public class OrderRow
    {
        public int OrderRowId {  get; set; }

        public int Quantity {  get; set; }
        [Required]
        public decimal UnitPrice {  get; set; }
        public int OrderId {  get; set; }
        public Order Order { get; set; } = new Order(); 

        public int ProductId {  get; set; }

        public Product Product { get; set; }






    }
}
