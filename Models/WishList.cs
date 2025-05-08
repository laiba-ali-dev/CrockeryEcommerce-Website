using System.ComponentModel.DataAnnotations;

namespace ecommerce.Models
{
    public class WishList
    {
        [Key]
        public int WishListId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
