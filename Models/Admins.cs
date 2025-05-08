using System.ComponentModel.DataAnnotations;

namespace ecommerce.Models
{
    public class Admins
    {
        [Key]
        public int AdminId { get; set; }

		[Required(ErrorMessage = "Name is required")]
		[MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
		[MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
		[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces")]
		public string Username { get; set; }

		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Phone is required")]
		[RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number is incorrect")]
		public string PhoneNumber { get; set; }

		[Required(ErrorMessage = "Password is required")]
		[MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
		[MaxLength(50, ErrorMessage = "Password cannot exceed 50 characters")]
		[RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
			 ErrorMessage = "Password must contain at least one letter, one number, and one special character")]
		public string Password { get; set; }


    }
}
