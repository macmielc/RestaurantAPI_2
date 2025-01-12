using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI_2.Entities;
using RestaurantAPI_2.Models;

namespace RestaurantAPI_2.Services
{
    public class RestaurantService : IRestaurantService
    {
        private RestaurantDBContext _dbCOntext;
        private IMapper _mapper;

        public RestaurantService(RestaurantDBContext dBContext, IMapper mapper)
        {
            _dbCOntext = dBContext;
            _mapper = mapper;
        }


        public RestaurantDto GetById(int id)
        {
            var restaurant = _dbCOntext.Restaurants.
                Include(r => r.Address).
                Include(r => r.Dishes).
                FirstOrDefault(r => r.Id == id);

            if (restaurant is null) return null;

            var restaurantDto = _mapper.Map<RestaurantDto>(restaurant);

            return restaurantDto;
        }

        public IEnumerable<RestaurantDto> GetAll()
        {
            var restaurants = _dbCOntext.Restaurants
                .Include(r => r.Address) // Dodawanie do encji tabele powiązanych (klucze obce i obiekty) np dania (Dishes) i adresy (Address) 
                .Include(r => r.Dishes)
                .ToList();

            var restaurantDtos = _mapper.Map<List<RestaurantDto>>(restaurants);

            return restaurantDtos;
        }


        public int Create(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);

            _dbCOntext.Restaurants.Add(restaurant);
            _dbCOntext.SaveChanges();

            return restaurant.Id;
        }
    }
}
