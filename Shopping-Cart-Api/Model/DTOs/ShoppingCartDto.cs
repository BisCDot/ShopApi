namespace Shopping_Cart_Api.Model.DTOs
{
    public class ShoppingCartDto
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }
        public List<CartDetailDto> CartDetails { get; set; }
    }
}