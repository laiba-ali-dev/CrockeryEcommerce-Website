using System.ComponentModel.DataAnnotations;

namespace ecommerce.Models
{
    public class ProductsViewModel
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public string Stock { get; set; }

        [Required]
        public IFormFile Photo { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string CreateDate { get; set; }
    }
}
