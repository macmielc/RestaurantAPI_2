using Microsoft.AspNetCore.Http.Extensions;
using RestaurantAPI_2.Exceptions;
using System.Diagnostics;

namespace RestaurantAPI_2.Middlewere
{
    public class RequestTimeMiddleware : IMiddleware
    {
        ILogger<RequestTimeMiddleware> _logger;
        Stopwatch sw;

        public RequestTimeMiddleware(ILogger<RequestTimeMiddleware> logger)
        {
            _logger = logger;
            sw = Stopwatch.StartNew();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            sw.Start();
            //Thread.Sleep(5000);
            await next.Invoke(context);
            sw.Stop();

            if (sw.ElapsedMilliseconds > 4000)
            {
                _logger.LogInformation($"Request {context.Request.Method} {context.Request.Path} took {sw.ElapsedMilliseconds} ms");
            }
        }
    }
}
