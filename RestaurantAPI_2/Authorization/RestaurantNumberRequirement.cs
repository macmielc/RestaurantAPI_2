using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI_2.Authorization
{
    public class RestaurantNumberRequirement : IAuthorizationRequirement
    {
        public RestaurantNumberRequirement(int v)
        {
            this.MinCount = v;
        }

        public int MinCount { get; }
    }
}
