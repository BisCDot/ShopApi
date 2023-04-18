using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Shopping_Cart_Api.Data;
using Shopping_Cart_Api.Model;
using Shopping_Cart_Api.ViewModels;

namespace Shopping_Cart_Api.Service
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> user)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = user;
        }

        public async Task<string> AddOrder(OrderViewModel order)
        {
            string userId = GetUserId();
            var entity = new Order()
            {
                Id = Guid.NewGuid(),
                CustomerAddress = order.Address,
                CustomerEmail = order.Email,
                CustomerName = order.CustomerName,
                CustomerPhone = order.Phone,
                OrderDate = DateTime.Now,
                PaymentType = order.PaymentType,
                Status = order.Status
            };
            _context.Orders.Add(entity);

            foreach (var item in order.OrderDetails)
            {
                var orderDetails = new OrderDetail()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    OrderId = entity.Id,
                    ProductDetailId = new Guid(item.ProductDetailId),
                    Price = item.Price,
                    Quantity = item.Quantity,
                };
                _context.OrderDetails.Add(orderDetails);
            }
            bool success = await _context.SaveChangesAsync() == 1 + order.OrderDetails.Count;
            if (success) return entity.Id.ToString();
            else return "order failed";
        }

        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userName = _userManager.GetUserId(principal);
            var user = _userManager.FindByNameAsync(userName);
            string userId = user.Result.Id;
            return userId;
        }
    }
}