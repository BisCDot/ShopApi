using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Shopping_Cart_Api.Model
{
    public class ShoppingCart
    {
        [Key]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<CartDetail> CartDetail { get; set; }
    }
}