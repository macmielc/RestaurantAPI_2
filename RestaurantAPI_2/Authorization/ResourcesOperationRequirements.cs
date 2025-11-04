using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Client;

namespace RestaurantAPI_2.Authorization
{
    public enum ResourcesOperation
    {
        Create,
        Read,
        Update, 
        Delete
    }
    public class ResourcesOperationRequirements : IAuthorizationRequirement
    {
        public ResourcesOperationRequirements(ResourcesOperation operation)
        {
            ResourcesOperation = operation;
        }

        public ResourcesOperation ResourcesOperation { get; }
    }
}
