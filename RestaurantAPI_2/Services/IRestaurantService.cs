using RestaurantAPI_2.Models;
using System.Security.Claims;

namespace RestaurantAPI_2.Services
{
    public interface IRestaurantService
    {
        RestaurantDto GetById(int id);
        PageResult<RestaurantDto> GetAll(RestaurantQuery query);

        int Create(CreateRestaurantDto dto);

        void Delete(int id);

        void Update(UpdateRestaurantDto dto, int id);
    }
}