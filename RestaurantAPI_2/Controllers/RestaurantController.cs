using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI_2.Entities;
using RestaurantAPI_2.Models;
using RestaurantAPI_2.Services;

namespace RestaurantAPI_2.Controllers
{
    [Route("api/restaurant")]
    public class RestaurantController : ControllerBase
    {
        private IRestaurantService _restaruantService;

        public RestaurantController(IRestaurantService restaruantService)
        {
            _restaruantService = restaruantService;
        }
        [HttpPost]
        public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto)
        {
            // Walidowanie porpawności danych przesyłanych do zapisania
            if (!ModelState.IsValid)
            { 
                return BadRequest(ModelState);
            }

            int id = _restaruantService.Create(dto);
            // Zwracanie wyników z url do danychy do zapytania o stworzona restauracje i zapisaną na BD 
            return Created($"/app/restaurant/{id}", null);
        }


        [HttpGet()]
        public ActionResult<IEnumerable<RestaurantDto>> GetAll()
        {
            var restaurantDtos = _restaruantService.GetAll();

            return Ok(restaurantDtos);
        }

        [HttpGet("{id}")]
        public ActionResult<RestaurantDto> Get([FromRoute] int id)
        {
            var restaurant = _restaruantService.GetById(id);
            // Weryfikacja czy restauracja nie jest null
            if (restaurant is null)
            {
                return NotFound();
            }

            return Ok(restaurant);
        }
    } 
}
