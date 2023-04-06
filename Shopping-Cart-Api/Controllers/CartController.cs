using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Shopping_Cart_Api.Data;
using Shopping_Cart_Api.Extensions;
using Shopping_Cart_Api.Model;
using Shopping_Cart_Api.Model.DTOs;
using Shopping_Cart_Api.Service;
using Shopping_Cart_Api.ViewModels;
using System.Security.Claims;
using System.Text;
using System.Threading;

namespace Shopping_Cart_Api.Controllers
{
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private string cartListCacheKey = "cartList";
        private readonly IDistributedCache _cache;
        private readonly ILogger<CartController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartService _cart;
        private readonly ApplicationDbContext _context;
        private static readonly SemaphoreSlim semaphore = new(1, 1);

        public CartController(IDistributedCache cache, ILogger<CartController> logger, UserManager<ApplicationUser> userManager, ICartService cart, ApplicationDbContext context)
        {
            _cache = cache;
            _logger = logger;
            _userManager = userManager;
            _cart = cart;
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            if (_cache.TryGetValue(cartListCacheKey, out List<ShoppingCartDto>? cartCaches))
            {
                _logger.Log(LogLevel.Information, "cart list found in cache.");
                string startTimeString = "Not found.";
                var value = await _cache.GetAsync(cartListCacheKey);

                if (value != null)
                {
                    startTimeString = Encoding.UTF8.GetString(value);
                    var valueJson = JsonConvert.DeserializeObject<List<ShoppingCartDto>>(startTimeString);
                    return Ok(valueJson);
                }
            }
            else
            {
                try
                {
                    await semaphore.WaitAsync();
                    if (_cache.TryGetValue(cartListCacheKey, out cartCaches))
                    {
                        _logger.Log(LogLevel.Information, "cart list found in cache.");
                    }
                    else
                    {
                        var cartId = _cart.GetIdCart();
                        cartListCacheKey = cartId;
                        cartCaches = await _cart.GetCartItemsByUserId();
                        _logger.Log(LogLevel.Information, "Cart list not found in cache.Fetching from database.");
                        var cacheEntryOptions = new DistributedCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromSeconds(1800))
                            .SetAbsoluteExpiration(TimeSpan.FromSeconds(1800));
                        await _cache.SetAsync(cartListCacheKey, cartCaches, cacheEntryOptions);
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }
            return Ok(cartCaches);
        }

        [Authorize]
        [HttpPost("{qty}/{productDetailId}")]
        public async Task<IActionResult> AddCart(int qty, string productDetailId)
        {
            Guid guidOutput;
            bool isValid = Guid.TryParse(productDetailId, out guidOutput);
            if (qty == 0 && !isValid)
                return BadRequest("Invalid Inputs");
            var isSuccessResult = await _cart.AddCart(qty, guidOutput);
            var cartId = _cart.GetIdCart();
            cartListCacheKey = cartId;
            _cache.Remove(cartListCacheKey);

            if (isSuccessResult == 0)
                return BadRequest("Failed to add.");
            else
            {
                //var NewUri = Url.Link("ProductGet",new{id = new Guid(isSuccessResult)});
                //return Created(NewUri,model);
                return Ok(new { success = true, Count = isSuccessResult, message = "seccessful add" });
            }
        }
    }
}