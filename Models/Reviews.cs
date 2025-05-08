using System.ComponentModel.DataAnnotations;

namespace ecommerce.Models
{
    public class Reviews
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public string Ratings { get; set; }

        [Required]
        public string Comments { get; set; }

        [Required]
        public string AddedBy { get; set; }

    }
}
