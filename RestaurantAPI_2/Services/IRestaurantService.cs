using RestaurantAPI_2.Models;
using System.Security.Claims;

namespace RestaurantAPI_2.Services
{
    public interface IRestaurantService
    {
        RestaurantDto GetById(int id);
        IEnumerable<RestaurantDto> GetAll();

        int Create(CreateRestaurantDto dto, int userId);

        void Delete(int id, ClaimsPrincipal user);

        void Update(UpdateRestaurantDto dto, int id, ClaimsPrincipal user);
    }
}