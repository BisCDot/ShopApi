using Shopping_Cart_Api.Model;
using Shopping_Cart_Api.Model.DTOs;
using Shopping_Cart_Api.ViewModels;

namespace Shopping_Cart_Api.Service
{
    public interface ICartService
    {
        Task<int> AddCart(int qty, Guid productDetailId);

        Task<IEnumerable<ShoppingCartDto>> GetCartItemsByUserId();

        Task<ShoppingCart> GetCart(string userId);

        Task<bool> RemoveItemFromCart(Guid id);
        Task ClearShoppingCartAsync();

        string GetIdCart();
    }
}