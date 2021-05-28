using System;
using System.Collections.Generic;
using System.Text;

namespace SalesDbLib
{
    public class OrderLine
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Product { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal Price { get; set; }
        public Order Order { get; set; }
    }
}
