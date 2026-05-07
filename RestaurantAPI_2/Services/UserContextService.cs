using System.Security.Claims;

namespace RestaurantAPI_2.Services
{
    /// <summary>
    /// Udostępnianie informacji o użytkwoniku w wymagany kontekscie i udostępnanie danych o użytkwonkiu
    /// </summary>
    public class UserContextService : IUserContextService
    {
        public UserContextService(IHttpContextAccessor httpContextAccessor) 
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// Tylko jest dostępne gdy wymagany jest nagłowek autoryzacj stąd przy ? przy HttpContext
        /// </summary>
        public ClaimsPrincipal User => httpContextAccessor.HttpContext?.User;

        public int? GetUserId => User is null ? null : (int?)int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
    }
}
