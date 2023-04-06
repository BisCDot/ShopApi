using Shopping_Cart_Api.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping_Cart_Api.Model
{
    public class CartDetail
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ShoppingCartId { get; set; }

        [ForeignKey(nameof(ShoppingCartId))]
        public virtual ShoppingCart ShoppingCart { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public double UnitPrice { get; set; }

        [Required]
        public Guid ProductDetailId { get; set; }

        [ForeignKey(nameof(ProductDetailId))]
        public virtual ProductDetail ProductDetail { get; set; }
    }
}