using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI_2.Authorization;
using RestaurantAPI_2.Entities;
using RestaurantAPI_2.Exceptions;
using RestaurantAPI_2.Models;
using System.Security.Claims;

namespace RestaurantAPI_2.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDBContext _dbCOntext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;

        public RestaurantService(RestaurantDBContext dBContext, IMapper mapper, ILogger<RestaurantService> logger,
            IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _dbCOntext = dBContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
        }

        public RestaurantDto GetById(int id)
        {
            var restaurant = _dbCOntext.Restaurants.
                Include(r => r.Address).
                Include(r => r.Dishes).
                FirstOrDefault(r => r.Id == id);

            if (restaurant is null) throw new NotFoundException("Restaurant not found");

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
            restaurant.CreatedById = _userContextService.GetUserId;// było userId;
            _dbCOntext.Restaurants.Add(restaurant);
            _dbCOntext.SaveChanges();

            return restaurant.Id;
        }


        public void Delete(int id)
        {
            _logger.LogError($"Restaurant with id: {id} DELETE action invoked");

            var restaurant = _dbCOntext.Restaurants.
                FirstOrDefault(r => r.Id == id);


            if (restaurant is null) throw new NotFoundException("Restaurant not found");
            // weryfikacja czy użytkonik jest zautoryzowany do danej czynności
            // było             var authorizationResult = _authorizationService.AuthorizeAsync(user, restaurant, new ResourcesOperationRequirements(ResourcesOperation.Delete)).Result;
            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant, new ResourcesOperationRequirements(ResourcesOperation.Delete)).Result;
            // w przypadku braku autoryzacji zwracanie informacji 403
            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            _dbCOntext.Remove(restaurant);
            _dbCOntext.SaveChanges();
        }

        public void Update(UpdateRestaurantDto dto, int id)
        {
            

            var restaurant = _dbCOntext.Restaurants.
                FirstOrDefault(r => r.Id == id);

            if (restaurant is null) throw new NotFoundException("Restaurant not found");

            // było var authorizationResult = _authorizationService.AuthorizeAsync(user, restaurant, new ResourcesOperationRequirements(ResourcesOperation.Update)).Result;
            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant, new ResourcesOperationRequirements(ResourcesOperation.Update)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }
            // Zmiana wartości
            restaurant.Name = dto.Name;
            restaurant.Description = dto.Description;
            restaurant.HasDelivery = dto.HasDelivery;
            // Zaktualizowanie wartości
            _dbCOntext.Update(restaurant); // nie jest konieczne wystarczy samo _dbCOntext.SaveChanges();
            _dbCOntext.SaveChanges();
        }
    }
}
