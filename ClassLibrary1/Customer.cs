using System;

namespace SalesDbLib
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public decimal Sales { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
