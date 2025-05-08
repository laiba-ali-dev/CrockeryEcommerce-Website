using System.ComponentModel.DataAnnotations;

namespace ecommerce.Models
{
    public class Products
    {
        [Key]
        public int ProductId { get; set; }

		[Required(ErrorMessage = "Product Name is required")]
		[MinLength(2, ErrorMessage = "Product Name must be at least 2 characters")]
		[MaxLength(100, ErrorMessage = "Product Name cannot exceed 100 characters")]
		[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Description is required")]
		[MinLength(10, ErrorMessage = "Description must be at least 10 characters")]
		[MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
		public string Description { get; set; }


		[Required(ErrorMessage = "Price is required")]
		[Range(1, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
		public int Price { get; set; }

		[Required(ErrorMessage = "Stock is required")]
		[Range(0, int.MaxValue, ErrorMessage = "Stock must be 0 or greater")]
		public string Stock { get; set; }

		[Required(ErrorMessage = "Product Image is required")]
		[Url(ErrorMessage = "Invalid Image URL format")]
		public string ImagePath { get; set; }

		[Required(ErrorMessage = "Category is required")]
		[MaxLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
		public string Category { get; set; }

		[Required(ErrorMessage = "Create Date is required")]
		public string CreateDate { get; set; }
    }
}
