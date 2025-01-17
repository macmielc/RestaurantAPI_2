using RestaurantAPI_2.Models;

namespace RestaurantAPI_2.Services
{
    public interface IDishService
    {
        int Create(int restaurantId, CreateDishDto dto);
    }
}
