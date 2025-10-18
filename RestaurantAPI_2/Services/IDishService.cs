using RestaurantAPI_2.Models;

namespace RestaurantAPI_2.Services
{
    public interface IDishService
    {
        int Create(int restaurantId, CreateDishDto dto);

        DishDto GetById(int restaurantId, int dishId);

        List<DishDto> GetAll(int restaurantId);
        void RemoveAll(int restaurantId);

        void Remove(int restaurantId, int dishId);
    }
}
