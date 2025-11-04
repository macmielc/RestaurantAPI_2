using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace RestaurantAPI_2.Authorization
{
    public class MinimumAgeRequirementHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        public MinimumAgeRequirementHandler(ILogger<MinimumAgeRequirementHandler> logger) 
        { 
            _logger = logger;
        }

        private readonly ILogger<MinimumAgeRequirementHandler> _logger;

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            var dateOfBirth = DateTime.Parse(context.User.FindFirst(c => c.Type == "DateOfBirth").Value);
            // Pobieranie z claimu 
            var userEmail = context.User.FindFirst(c => c.Type ==ClaimTypes.Name).Value;
            // Zapisywanie infromacji do logera
            _logger.LogInformation($"User: {userEmail} with date of birth: [{dateOfBirth}]");

            var date20 = dateOfBirth.AddYears(requirement.MinimumAge);

            if (dateOfBirth.AddYears(requirement.MinimumAge) <= DateTime.Today)
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
