using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace ecommerce.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

		[Required(ErrorMessage = "Name is required")]
		[MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
		[MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
		[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid email format")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Password is required")]
		[MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
		[MaxLength(50, ErrorMessage = "Password cannot exceed 50 characters")]
		[RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
			 ErrorMessage = "Password must contain at least one letter, one number, and one special character")]
		public string Password { get; set; }

		[Required(ErrorMessage = "Phone Number is required")]
		[RegularExpression(@"^\d{11}$", ErrorMessage = "Phone Number is incorrect")]
		public string PhoneNumber { get; set; }


    }
}
