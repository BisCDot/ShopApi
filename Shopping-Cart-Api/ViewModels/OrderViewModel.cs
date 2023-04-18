using System.ComponentModel.DataAnnotations;

namespace Shopping_Cart_Api.ViewModels
{
    public class OrderViewModel
    {
        [Required]
        public string Address { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string PaymentType { get; set; }

        public bool Status { get; set; }

        public List<OrderDetailsViewModel> OrderDetails { get; set; }
    }

    public class OrderDetailsViewModel
    {
        public string ProductDetailId { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
    }
}