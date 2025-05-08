using Humanizer;
using MailKit.Search;
using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailId { get; set; }

        [ForeignKey("Orders")]
        public int OrderId { get; set; }

        [ForeignKey("Products")]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int Price { get; set; }

        [NotMapped]
        public int TotalPrice => Quantity * Price;


        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Orders Order { get; set; }
        public virtual Products Product { get; set; }









    }
}
