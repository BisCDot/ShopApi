using Shopping_Cart_Api.ViewModels;

namespace Shopping_Cart_Api.Service
{
    public interface IOrderService
    {
        Task<string> AddOrder(OrderViewModel model);
    }
}