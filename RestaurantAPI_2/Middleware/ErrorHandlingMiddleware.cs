
using RestaurantAPI_2.Exceptions;

namespace RestaurantAPI_2.Middlewere
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (BadRequestException ex)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(ex.Message);
            }
            catch (NotFoundException ex)
            {
                // Dodawanie do logów informacji nie jest potrzebne
                // _logger.LogError(ex, ex.Message);

                context.Response.StatusCode = 404;
                //await context.Response.WriteAsync(ex.Message);
            }
            catch (ForbidException ex)
            {
                // Dodawanie do logów informacji nie jest potrzebne
                // _logger.LogError(ex, ex.Message);

                context.Response.StatusCode = 403;
                await context.Response.WriteAsync(ex.Message);
            }
            catch (Exception ex) 
            { 
                _logger.LogError(ex, ex.Message);

                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Something went wrong");
            }

        }
    }
}
