namespace ecommerce.Models
{
    public class CartViewModel
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;  // Calculate total price dynamically
    }
}
