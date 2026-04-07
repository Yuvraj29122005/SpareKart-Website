using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SpareKart_Website.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        [Required]
        public string ShippingAddress { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Processing, Shipped, Delivered

        // Payment fields
        public string PaymentMethod { get; set; } = "COD"; // COD or Online
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Completed, Failed
        public string? RazorpayOrderId { get; set; }
        public string? RazorpayPaymentId { get; set; }
        public string? RazorpaySignature { get; set; }

        // Customer contact info
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
