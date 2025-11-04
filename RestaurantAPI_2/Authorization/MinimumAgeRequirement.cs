using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI_2.Authorization
{
    public class MinimumAgeRequirement : IAuthorizationRequirement
    {
        public MinimumAgeRequirement(int v)
        {
            this.MinimumAge = v;
        }

        public int MinimumAge { get; }
    }
}