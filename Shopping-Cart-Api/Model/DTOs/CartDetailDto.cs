using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Shopping_Cart_Api.Model.DTOs
{
    public class CartDetailDto
    {
        public Guid Id { get; set; }

        public Guid ShoppingCartId { get; set; }
        public string UserId { get; set; }

        public int Quantity { get; set; }

        public double UnitPrice { get; set; }

        public Guid ProductDetailId { get; set; }
        public string Size { get; set; }
        public int Stock { get; set; }

        public Guid ProductId { get; set; }
        public string ProductName { get; set; }

        public double Price { get; set; }
        public string Path { get; set; }
    }
}