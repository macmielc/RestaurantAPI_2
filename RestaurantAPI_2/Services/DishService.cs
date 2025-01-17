using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI_2.Entities;
using RestaurantAPI_2.Exceptions;
using RestaurantAPI_2.Models;

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


        public int Create(int restaurnatId, CreateDishDto dto)
        {

            var restaurant = _dbCOntext.Restaurants.FirstOrDefault(r => r.Id == restaurnatId);

            if (restaurant == null) 
                throw new NotFoundException("Restaurant not found");

            var dishEntity = _mapper.Map<Dish>(dto);
            // Powiązanie dania z restauracją
            dishEntity.RestaurantId = restaurnatId;
            // Zapisywanie dania na DB
            _dbCOntext.Dishes.Add(dishEntity);
            _dbCOntext.SaveChanges();   

            return dishEntity.Id;
        }
    }
}
