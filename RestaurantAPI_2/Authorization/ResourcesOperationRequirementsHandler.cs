using Microsoft.AspNetCore.Authorization;
using RestaurantAPI_2.Entities;
using System.Security.Claims;

namespace RestaurantAPI_2.Authorization
{
    public class ResourcesOperationRequirementsHandler : AuthorizationHandler<ResourcesOperationRequirements, Restaurant>
    {
        public ResourcesOperationRequirementsHandler()
        {

        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourcesOperationRequirements requirement, Restaurant restaurant)
        {
            if (requirement.ResourcesOperation == ResourcesOperation.Create || requirement.ResourcesOperation == ResourcesOperation.Read)
            {
                context.Succeed(requirement);
            }

            var userId = context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;

            if (restaurant.CreatedById == int.Parse(userId))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
