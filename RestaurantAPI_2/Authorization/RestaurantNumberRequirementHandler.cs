using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI_2.Entities;
using System.Security.Claims;

namespace RestaurantAPI_2.Authorization
{
    public class RestaurantNumberRequirementHandler : AuthorizationHandler<RestaurantNumberRequirement>
    {
        public RestaurantNumberRequirementHandler( RestaurantDBContext dBContext, ILogger<RestaurantNumberRequirement> logger)
        {
            _logger = logger;
            _dBContext = dBContext;
        }

        private readonly ILogger<RestaurantNumberRequirement> _logger;
        private readonly RestaurantDBContext _dBContext;

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RestaurantNumberRequirement requirement)
        {
            var userId = int.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var userEmail = context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var count = _dBContext.Restaurants.Count(r => r.CreatedById == userId);
            // Zapisywanie infromacji do logera
            _logger.LogInformation($"User: {userEmail} number of restaurant: [{count}]");


            if (count >= requirement.MinCount)
            {
                _logger.LogInformation($"Authorization succeded");
                context.Succeed(requirement);
            }
            else 
            {
                _logger.LogInformation($"Authorization failed");
            }

            return Task.CompletedTask;
        }
    }
}
