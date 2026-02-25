using System.Collections.Generic;

namespace SpareKart_Website.Models
{
    public class OrderModel
    {
        public string OrderId { get; set; }
        public string Date { get; set; }
        public int TotalAmount { get; set; }
        public string Address { get; set; }
        public List<CartItem> Items { get; set; }
    }
}