using System.Security.Claims;

namespace RestaurantAPI_2.Services
{
    public interface IUserContextService
    {
        int? GetUserId {  get; }

        ClaimsPrincipal User { get; }
    }
}
