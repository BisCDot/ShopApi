using AutoMapper.QueryableExtensions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Cart_Api.Data;
using Shopping_Cart_Api.Model;
using Shopping_Cart_Api.Model.DTOs;
using Shopping_Cart_Api.ViewModels;
using System.Security.Cryptography.X509Certificates;
using Shopping_Cart_Api.Helpers;

namespace Shopping_Cart_Api.Service
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public CartService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<int> AddCart(int qty, Guid productDetailId)
        {
            var message = "add cart failed";
            string userId = GetUserId();
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception("user is not login");
                }
                //var cart = await GetCart(userId);
                var exist = await _context.ShoppingCarts.AnyAsync(i => i.UserId == userId);

                if (!exist)
                {
                    var shoppingCart = new ShoppingCart() { Id = Guid.NewGuid(), UserId = userId };
                    _context.ShoppingCarts.Add(shoppingCart);
                }
                _context.SaveChanges();
                var cart = await GetCart(userId);

                var cartItem = _context.CartDetails.FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.ProductDetailId == productDetailId);
                if (cartItem != null)
                {
                    cartItem.Quantity += qty;
                }
                else
                {
                    var product = _context.ProductDetails.Include(a => a.Products).FirstOrDefault(i => i.Id == productDetailId);
                    cartItem = new CartDetail()
                    {
                        ProductDetailId = productDetailId,
                        ShoppingCartId = cart.Id,
                        Quantity = qty,
                        UnitPrice = product.Products.Price,
                    };
                    _context.Add(cartItem);
                }
                _context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                throw;
            }
            var cartItemCount = await GetCartItemCount();
            return cartItemCount;
        }

        public async Task<bool> RemoveItemFromCart(Guid ItemCartId)
        {
            var shoppingCartItem = _context.CartDetails.FirstOrDefault(n => n.Id == ItemCartId);
            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.Quantity > 1)
                {
                    shoppingCartItem.Quantity--;
                }
                else
                {
                    _context.CartDetails.Remove(shoppingCartItem);
                }
            }
            return 1 == _context.SaveChanges();
        }

        public async Task<int> GetCartItemCount(string userId = "")
        {
            if (!string.IsNullOrEmpty(userId))
            {
                userId = GetUserId();
            }
            var data = await (from cart in _context.ShoppingCarts
                              join cartDetail in _context.CartDetails
                              on cart.Id equals cartDetail.ShoppingCartId
                              select new { cartDetail.Id }
                        ).ToListAsync();
            return data.Count;
        }

        public async Task<ShoppingCart> GetCart(string userId)
        {
            var cart = await _context.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;
        }

        public async Task<IEnumerable<ShoppingCartDto>> GetCartItemsByUserId()
        {
            var userId = GetUserId();
            if (userId == null)
                throw new Exception("Invalid userid");

            var items = await _context.CartDetails.Where(a => a.ShoppingCart.UserId == userId).ProjectTo<CartDetailDto>(_mapper.ConfigurationProvider).AsNoTracking().ToListAsync();
            var group = items.GroupBy(i => i.ShoppingCartId);
            var result = new List<ShoppingCartDto>();
            foreach (var item in group)
            {
                result.Add(new ShoppingCartDto
                {
                    Id = item.First().Id,
                    UserId = item.First().UserId,
                    CartDetails = item.ToList()
                });
            }

            return result;
        }

        public string GetIdCart()
        {
            var userId = GetUserId();
            var cart = GetCart(userId);
            string cartId = cart.Result.Id.ToString();
            return cartId;
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