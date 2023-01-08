using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Shopping_Cart_Api.Model.Repository;
using Shopping_Cart_Api.Model;
using System.Net;
using System.Text;
using Shopping_Cart_Api.Service;
using System.Text.Json.Serialization;
using System.Text.Json;
using Newtonsoft.Json;

namespace Shopping_Cart_Api.Controllers
{
    [Route("api/[controller]")]
    public class CartsController : ControllerBase
    {
        private const string cartListCacheKey = "cartList";
        private readonly IDataRepository<CartCache> _dataRepository;
        private readonly IDistributedCache _cache;
        private readonly IProductService _productService;
        private readonly ILogger<CartsController> _logger;
        private static readonly SemaphoreSlim semaphore = new(1, 1);

        public CartsController(IDataRepository<CartCache> dataRepository, IDistributedCache cache, ILogger<CartsController> logger, IProductService productService)
        {
            _dataRepository = dataRepository ?? throw new ArgumentNullException(nameof(dataRepository));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _productService = productService;
        }

        [HttpGet("{id}", Name = "CartGet")]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            if (_cache.TryGetValue(cartListCacheKey, out Product? cartCaches))
            {
                _logger.Log(LogLevel.Information, "cart list found in cache.");
                string startTimeString = "Not found.";
                var value = await _cache.GetAsync(cartListCacheKey);

                if (value != null)
                {
                    startTimeString = Encoding.UTF8.GetString(value);
                    var valueJson = JsonConvert.DeserializeObject<Product>(startTimeString);
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
                        _logger.Log(LogLevel.Information, "Cart list not found in cache.Fetching from database.");
                        cartCaches = await _productService.GetProductById(id);
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

        [HttpPost]
        public IActionResult Post([FromBody] CartCache cartCache)
        {
            if (cartCache == null)
            {
                return BadRequest("Cart is null");
            }
            _dataRepository.Add(cartCache);
            _cache.Remove(cartListCacheKey);
            return new ObjectResult(cartCache) { StatusCode = (int)HttpStatusCode.Created };
        }
    }
}