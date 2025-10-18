using Microsoft.AspNetCore.Mvc;
using RestaurantAPI_2.Entities;
using RestaurantAPI_2.Models;
using RestaurantAPI_2.Services;

namespace RestaurantAPI_2.Controllers
{
    [Route("api/restaurant/{restaurantId}/dish")]
    [ApiController] // Automatyczna walidacja kontrolera
    public class DishController : ControllerBase
    {
        private IDishService _dishService;

        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }

        [HttpPost]
        public ActionResult Post([FromRoute] int restaurantId, [FromBody] CreateDishDto dto)
        {
            var newDishId = _dishService.Create(restaurantId, dto);

            return Created($"api/restaurant/{restaurantId}/dish/{newDishId}", null);

        }

        [HttpGet("{dishId}")]
        public ActionResult<DishDto> Get([FromRoute] int restaurantId, [FromRoute] int dishId)
        {

            DishDto dishDto = _dishService.GetById(restaurantId, dishId);

            return dishDto;
        }

        [HttpGet]
        public ActionResult<List<DishDto>> GetAll([FromRoute] int restaurantId)
        {
            var newDishId = _dishService.GetAll(restaurantId);

            return newDishId;

        }


        [HttpDelete]
        public ActionResult Delete([FromRoute] int restaurantId)
        {
            _dishService.RemoveAll(restaurantId);

            return NoContent();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="restaurantId"></param>
        /// <param name="dishId"></param>
        /// <returns></returns>
        /// <remarks> Rozwiązanie problemu sqggera przy braku deklaracji przy usuwnaiu jak ma być trakotwane <paramref name="dishId"/>
        ///  https://www.google.com/search?q=Fetch+error+response+status+is+500+%2Fswagger%2Fv1%2Fswagger.json&oq=Fetch+error+response+status+is+500+%2Fswagger%2Fv1%2Fswagger.json&gs_lcrp=EgZjaHJvbWUyBggAEEUYOdIBCDMwMTRqMGo3qAIAsAIA&sourceid=chrome&ie=UTF-8#fpstate=ive&vld=cid:d7128538,vid:f71EFLrKLFM,st:0
        /// </remarks>
        [HttpDelete("{dishId}")] 
        public ActionResult Delete([FromRoute] int restaurantId,[FromRoute] int dishId)
        {
            _dishService.RemoveAll(restaurantId);

            return NoContent();

        }
    }
}
