namespace SpareKart_Website.Models
{
    public class CartItem
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
        public int MaxStock { get; set; } = 10;
    }
}