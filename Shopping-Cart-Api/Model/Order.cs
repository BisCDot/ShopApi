using System.ComponentModel.DataAnnotations;

namespace Shopping_Cart_Api.Model
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string CustomerAddress { get; set; }

        [Required]
        public string CustomerEmail { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string CustomerPhone { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public string PaymentType { get; set; }

        [Required]
        public bool Status { get; set; }
    }
}