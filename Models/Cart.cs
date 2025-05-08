using System.ComponentModel.DataAnnotations;

namespace ecommerce.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal  TotalPrice { get; set; }


    }
}
