using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI_2.Entities;
using RestaurantAPI_2.Models;
using RestaurantAPI_2.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace RestaurantAPI_2.Controllers
{
    [Route("api/restaurant")]
    [ApiController] // jezeli przyjdzie jakiekolwiek zapytanie automatycznie zostanie zwrócona informacja o błędach walidacji. Dlatego można usunąć kod !ModelState.IsValid
    //[Authorize] // < autoryzacja dla całego kontrolera
    public class RestaurantController : ControllerBase
    {
        private IRestaurantService _restaruantService;

        public RestaurantController(IRestaurantService restaruantService )
        {
            _restaruantService = restaruantService;
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto)
        {
            // w związku z wdrożeniem IUserContextService int userId = int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            int id = _restaruantService.Create(dto);
            // Zwracanie wyników z url do danychy do zapytania o stworzona restauracje i zapisaną na BD 
            return Created($"/app/restaurant/{id}", null);
        }

        [HttpGet()]
        [Authorize(Roles ="Admin,Manager")]
        //[Authorize(Policy = "HasNationality")]
        //[Authorize(Policy = "AtLeast20")]
        [Authorize(Policy = "MinRestaurantNr")]
        public ActionResult<IEnumerable<RestaurantDto>> GetAll()
        {
            var restaurantDtos = _restaruantService.GetAll();

            return Ok(restaurantDtos);
        }

        [HttpGet("{id}")] 
        //[AllowAnonymous] // < nie podelga autoryzacji wynikającej z wymogu nałożonego na cała klasę
        public ActionResult<RestaurantDto> Get([FromRoute] int id)
        {
            var restaurant = _restaruantService.GetById(id);
            // Zwracanie wyników. Brak restauracji o danym id jest obsługiwamny w metodzie serwisu GetById(id);
            return Ok(restaurant);
        }

        [HttpDelete("{id}")]
        public ActionResult<RestaurantDto> Delete([FromRoute] int id)
        {
            _restaruantService.Delete(id);
            // Weryfikacja czy restauracja została usunięta. Brak restauracji o danym id jest obsługiwamny w metodzie serwisu Delete(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult Update([FromBody] UpdateRestaurantDto dto, [FromRoute] int id)
        {
            // Weryfikacja zapisania zmian związanych z edycją restauracji
            _restaruantService.Update(dto, id);
            // Zwracanie informacji o powodzeniu zapisu edycji reatauracji. Brak restauracji o danym id jest obsługiwamny w metodzie serwisu Update(dto, id);
            return Ok();
        }
    } 
}
