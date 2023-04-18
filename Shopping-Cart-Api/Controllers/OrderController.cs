using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping_Cart_Api.Service;
using Shopping_Cart_Api.ViewModels;

namespace Shopping_Cart_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _order;
        private readonly ICartService _cart;

        public OrderController(IOrderService order, ICartService cart)
        {
            _order = order;
            _cart = cart;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderViewModel model)
        {
            var isSuccessResult = await _order.AddOrder(model);
            bool isGuid = Guid.TryParse(isSuccessResult, out Guid GuidOutput);
            if (!isGuid)
            {
                return BadRequest(isSuccessResult);
            }
            else
            {
                return Ok(new { success = true, message = "seccessfull order" });
            }
        }
    }
}