using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Shopping_Cart_Api.Attributes;
using Shopping_Cart_Api.Model;
using Shopping_Cart_Api.Model.Repository;
using Shopping_Cart_Api.Service;
using Shopping_Cart_Api.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping_Cart_Api.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private const string cartListCacheKey = "cartList";
        private readonly IProductService _productService;
        private readonly IDistributedCache _cache;

        public ProductsController(IProductService productService, IDistributedCache cache)
        {
            _productService = productService;
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var isSuccessResult = await _productService.GetAllProduct();
            if (isSuccessResult == null)
                return BadRequest();
            return Json(isSuccessResult);
        }

        [HttpGet("{id}", Name = "ProductGet")]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var isSuccessResult = await _productService.GetProductById(id);
            if (isSuccessResult == null) return BadRequest();
            return new JsonResult(isSuccessResult);
        }

        [Authorize(Policy = nameof(Constants.AdministratorRole))]
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] ProductViewModel model)
        {
            var isSuccessResult = await _productService.AddProduct(model);
            _cache.Remove(cartListCacheKey);

            //check if returned result is guid or not
            //if guid it was successfull. Otherwise unsuccessfull
            Guid GuidOutput;
            bool isGuid = Guid.TryParse(isSuccessResult, out GuidOutput);

            if (!isGuid)
                return BadRequest(isSuccessResult);
            else
            {
                //var NewUri = Url.Link("ProductGet",new{id = new Guid(isSuccessResult)});
                //return Created(NewUri,model);
                return Ok(new { success = true, message = "seccessful add" });
            }
        }

        [Authorize(Policy = nameof(Constants.AdministratorRole))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var isSuccessResult = await _productService.DeleteProductById(id);

            if (!isSuccessResult)
                return BadRequest();
            else
            {
                return Ok(new { success = true, message = "successful delete" });
            }
        }
    }
}