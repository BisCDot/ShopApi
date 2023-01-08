using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shopping_Cart_Api.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping_Cart_Api.Attributes
{
    public class CacheAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveSeconds;

        public CacheAttribute(int timeToLiveSeconds)
        {
            _timeToLiveSeconds = timeToLiveSeconds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //var cacheConfiguration = context.HttpContext.RequestServices.GetRequiredService<RedisConfiguration>();
            //if (!cacheConfiguration.Enabled)
            //{
            //    await next();
            //    return;
            //}
            //var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
            //var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            //var cacheResponse = await cacheService.GetCacheResponseAsync(cacheKey);
            //if (!string.IsNullOrEmpty(cacheResponse))
            //{
            //    var contentResult = new ContentResult
            //    {
            //        Content = cacheResponse,
            //        ContentType = "application/json",
            //        StatusCode = 200
            //    };
            //    context.Result = contentResult;
            //    return;
            //}
            //var excutedContext = await next();
            //if (excutedContext.Result is OkObjectResult objectResult)
            //{
            //    await cacheService.SetCacheResponseAsync(cacheKey, objectResult.Value, TimeSpan.FromSeconds(_timeToLiveSeconds));
            //}
        }

        private static string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append($"{request.Path}");
            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }
            return keyBuilder.ToString();
        }
    }
}