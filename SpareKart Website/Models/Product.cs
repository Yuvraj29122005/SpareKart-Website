using System.ComponentModel.DataAnnotations;

namespace SpareKart_Website.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public string Category { get; set; }

        public int StockQty { get; set; } = 10; // Default to 10 for existing products without it
    }
}
