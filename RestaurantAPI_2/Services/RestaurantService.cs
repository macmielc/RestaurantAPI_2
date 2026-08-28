using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI_2.Authorization;
using RestaurantAPI_2.Entities;
using RestaurantAPI_2.Exceptions;
using RestaurantAPI_2.Models;
using System.Linq.Expressions;
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
        /// <summary>
        /// Wyszukianie restauracji po id i zwrócenie jej w postaci obiektu RestaurantDto
        /// </summary>
        /// <param name="id"><see cref="int"/> id restauracji</param>
        /// <returns><see cref="RestaurantDto"/></returns>
        /// <exception cref="NotFoundException"></exception>
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
        // public IEnumerable<RestaurantDto> GetAll(RestaurantQuery query)
        /// <summary>
        /// Wyszukiwanie restauracji po nazwie lub opisie i zwrócenie ich w postaci obiektu <see cref="PageResult{RestaurantDto}"/>
        /// </summary>
        /// <param name="query"><see cref="RestaurantQuery"/></param>
        /// <returns><see cref="PageResult{RestaurantDto}"/></returns>
        public PageResult<RestaurantDto> GetAll(RestaurantQuery query)
        {
            var baseQuery = _dbCOntext.Restaurants
                .Include(r => r.Address) // Dodawanie do encji tabele powiązanych (klucze obce i obiekty) np dania (Dishes) i adresy (Address) 
                .Include(r => r.Dishes)
                // Kwerenda na podstawie wyrażenia po którym mają być przeszukiwane dane
                .Where(r => query.SearchPhrase == null || (r.Name.ToLower().Contains(query.SearchPhrase.ToLower()) || r.Description.ToLower().Contains(query.SearchPhrase.ToLower())));
            if(query.SortBy != null)
            {
                var columnsSelector = new Dictionary<string, Expression<Func<Restaurant, object>>>
                {
                    { nameof(Restaurant.Name), r => r.Name },
                    { nameof(Restaurant.Description), r => r.Description },
                    { nameof(Restaurant.Address), r => r.Address },
                    { nameof(Restaurant.AddressID), r => r.AddressID },
                    { nameof(Restaurant.Category), r => r.Category },
                    { nameof(Restaurant.ContactEmail), r => r.ContactEmail },
                    { nameof(Restaurant.ContactNumber), r => r.ContactNumber },
                    { nameof(Restaurant.CreatedById), r => r.CreatedById },
                    { nameof(Restaurant.HasDelivery), r => r.HasDelivery },
                    { nameof(Restaurant.Id), r => r.Id },

                };

                var selectedColumn = columnsSelector[query.SortBy];

                baseQuery = query.sortDirection == SortDirection.ASC ?
                    baseQuery.OrderBy(selectedColumn) :
                    baseQuery.OrderByDescending(selectedColumn);
            }


            var restaurants = baseQuery
                // Wybieranie strony do prezentacji
                .Skip((query.PageNumber -1) * query.PageSize )
                .Take(query.PageSize)
                .ToList();

            var restaurantDtos = _mapper.Map<List<RestaurantDto>>(restaurants);

            var result = new PageResult<RestaurantDto>(restaurantDtos, baseQuery.Count(), query.PageSize, query.PageNumber);

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public int Create(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.CreatedById = _userContextService.GetUserId;// było userId;
            _dbCOntext.Restaurants.Add(restaurant);
            _dbCOntext.SaveChanges();

            return restaurant.Id;
        }

        /// <summary>
        /// Usuwanie restauracji po id, weryfikacja czy użytkownik jest autoryzowany do danej czynności
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="ForbidException"></exception>
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
        /// <summary>
        /// Edycja restauracji po id, weryfikacja czy użytkownik jest autoryzowany do danej czynności
        /// </summary>
        /// <param name="dto"><see cref="UpdateRestaurantDto"/> - obiekt zawierający dane do aktualizacji restauracji</param>
        /// <param name="id"><see cref="int"/> id restauracji</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="ForbidException"></exception>
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
