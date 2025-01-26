using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI_2.Entities;
using RestaurantAPI_2.Exceptions;
using RestaurantAPI_2.Models;
using System.Linq;

namespace RestaurantAPI_2.Services
{
    public class DishService : IDishService
    {
        private readonly RestaurantDBContext _dbCOntext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;


        public DishService(RestaurantDBContext dBContext, IMapper mapper, ILogger<RestaurantService> logger)
        {
            _dbCOntext = dBContext;
            _mapper = mapper;
            _logger = logger;
        }


        public int Create(int restaurantId, CreateDishDto dto)
        {

            var restaurant = GetRestaurantById(restaurantId);

            if (restaurant == null) 
                throw new NotFoundException("Restaurant not found");

            var dishEntity = _mapper.Map<Dish>(dto);
            // Powiązanie dania z restauracją
            dishEntity.RestaurantId = restaurantId;
            // Zapisywanie dania na DB
            _dbCOntext.Dishes.Add(dishEntity);
            _dbCOntext.SaveChanges();   

            return dishEntity.Id;
        }


        public List<DishDto> GetAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            if (restaurant == null)
                throw new NotFoundException("Restaurant not found");

            var dishesDto = _mapper.Map<List<DishDto>>(restaurant.Dishes);

            return dishesDto;
        }

        public DishDto GetById(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            if (restaurant == null)
                throw new NotFoundException("Restaurant not found");

            var dish = _dbCOntext.Dishes.FirstOrDefault(d => d.Id == dishId);

            if (dish == null || dish.RestaurantId != restaurantId)
                throw new NotFoundException("Dish not found");

            var dishDto= _mapper.Map<DishDto>(dish);
            // Powiązanie dania z restauracją
            return dishDto;
        }


        public void Remove(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            //_logger.LogError($"Restaurant with id: {restaurantId} DELETE action invoked");

            if (restaurant is null) throw new NotFoundException("Restaurant not found");

            _dbCOntext.Remove(restaurant.Dishes);
            _dbCOntext.SaveChanges();
        }

        public void RemoveAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);
            //_logger.LogError($"Restaurant with id: {restaurantId} DELETE action invoked");
            
            if (restaurant is null) throw new NotFoundException("Restaurant not found");

            _dbCOntext.RemoveRange(restaurant.Dishes);
            _dbCOntext.SaveChanges();
        }

        /// <summary>
        /// Na potrzeby refaktroingu 
        /// </summary>
        /// <param name="restaurantId"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        private Restaurant GetRestaurantById(int restaurantId)
        {
            var restaurant = _dbCOntext.Restaurants
                .Include(r => r.Dishes)
                .FirstOrDefault(r => r.Id == restaurantId);

            if (restaurant == null)
                throw new NotFoundException("Restaurant not found");

            return restaurant;
        }
    }
}
