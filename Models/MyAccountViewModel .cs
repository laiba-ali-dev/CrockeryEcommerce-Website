namespace ecommerce.Models
{
    public class MyAccountViewModel
    {
        public User UserDetails { get; set; }
        public IEnumerable<Orders> orders { get; set; }
    }
}
