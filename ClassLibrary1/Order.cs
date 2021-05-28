using System;

namespace ClassLibrary1
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public Customer customer { get; set; }

    }
}
