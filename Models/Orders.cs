using System.ComponentModel.DataAnnotations;

namespace ecommerce.Models
{
    public class Orders
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public int UserId { get; set; }


        public int ProductId { get; set; }  // Foreign Key



        [Required(ErrorMessage = "First Name is required")]
		[MinLength(2, ErrorMessage = "First Name must be at least 2 characters")]
		[MaxLength(50, ErrorMessage = "First Name cannot exceed 50 characters")]
		[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Last Name is required")]
		[MinLength(2, ErrorMessage = "Last Name must be at least 2 characters")]
		[MaxLength(50, ErrorMessage = "Last Name cannot exceed 50 characters")]
		[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces")]
		public string LastName { get; set; }

		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Phone number is required")]
		[RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number is incorrect")]
		public string Phone { get; set; }

        [Required]
        public string OrderDate { get; set; }

		[Required(ErrorMessage = "Shipping Address is required")]
		[MinLength(5, ErrorMessage = "Shipping Address must be at least 5 characters")]
		[MaxLength(255, ErrorMessage = "Shipping Address cannot exceed 255 characters")]
		public string ShippingAddress { get; set; }

        [Required]
        public string Status { get; set; }

		[Required(ErrorMessage = "Payment Method is required")]
		[RegularExpression(@"^Cash on Delivery$", ErrorMessage = "Only 'Cash on Delivery' is allowed")]
		public string PaymentMethod { get; set; }


        [Required]
        public string TotalAmount { get; set; }


      

    }
}
