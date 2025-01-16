using RestaurantAPI_2.Models;

namespace RestaurantAPI_2.Services
{
    public interface IRestaurantService
    {
        RestaurantDto GetById(int id);
        IEnumerable<RestaurantDto> GetAll();

        int Create(CreateRestaurantDto dto);

        bool Delete(int id);

        bool Update(UpdateRestaurantDto dto, int id);
    }
}