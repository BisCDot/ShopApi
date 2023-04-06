using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping_Cart_Api.Model
{
    public class ProductDetail
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Size { get; set; }

        [Required]
        public int Quantity { get; set; }

        public Guid ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual Product Products { get; set; }

        public virtual ICollection<CartDetail> CartDetails { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}