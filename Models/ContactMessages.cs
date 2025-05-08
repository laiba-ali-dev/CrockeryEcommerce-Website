using System.ComponentModel.DataAnnotations;

namespace ecommerce.Models
{
    public class ContactMessages
    {
        [Key]
        public int MessageId { get; set; }

        [Required]
        public int UserId { get; set; }

		[Required(ErrorMessage = "Name is required")]
		[MinLength(2, ErrorMessage = "Name must be at least 2 characters")]
		[MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
		[RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Message is required")]
		[MinLength(10, ErrorMessage = "Message must be at least 10 characters")]
		[MaxLength(1000, ErrorMessage = "Message cannot exceed 1000 characters")]
		public string Message { get; set; }

        [Required]
        public string MessageDate { get; set; }




    }
}
