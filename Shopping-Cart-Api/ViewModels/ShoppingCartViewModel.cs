using Shopping_Cart_Api.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Shopping_Cart_Api.ViewModels
{
    public class ShoppingCartViewModel
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }
        public virtual List<CartDetail> CartDetail { get; set; }
    }
}