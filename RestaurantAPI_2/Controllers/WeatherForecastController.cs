using Microsoft.AspNetCore.Mvc;

namespace RestaurantAPI_2.Controllers
{
    [ApiController]
    [Route("[controller]")] // Placeholder - podstawiana jest nazwa kontrolera z klasy np WeatherForecastController 
    public class WeatherForecastController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherForcastService _service;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForcastService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var result = _service.Get();
            return result;
        }
        // Wywp³anie GET poprzez dodanie konstruktora Route
        /*
        [HttpGet]
        [Route("currentDay")]
        public IEnumerable<WeatherForecast> Get2()
        {
            var result = _service.Get();
            return result;
        }
        */
        // Wywp³anie GET poprzez dodanie konstruktora HttpGet + nazwa
        [HttpGet("currentDay/{max}")] // <- mapowanie na przekazywanie prametru przez Model Binder
        public IEnumerable<WeatherForecast> Get2([FromQuery] int take,[FromRoute]int max) // <- mo¿na dodac parametry FromQuery lub FromRoute
        {
            var result = _service.Get();
            return result;
        }

        [HttpPost] //[HttpPost("hello")]
        public ActionResult<string> Hello([FromBody] string name) // <- mo¿na dodac parametry FromQuery lub FromRoute
        {
            //HttpContext.Response.StatusCode = 401;

            //return StatusCode(401, $"Hello {name}")  ; // $"Hello {name}";

            return NotFound($"Hello {name} 2");
        }

        [HttpPost("generate")] // [HttpPost("generate/{take}")]
        public ActionResult<IEnumerable<WeatherForecast>> Generate([FromBody] TemperatureDemand weather, [FromQuery] int take)
        {
            if (!weather.ValidateTemperature || take < 0) return StatusCode(400, $"{ (!weather.ValidateTemperature ? "TemperaturaMax musi byæ wiêksza od TemperaturyMin\n" : "")}{(take < 0 ? "Liczba zwracanych elementów musi byæ wiêksza od 0" :"")}" );

            var result = Ok(_service.Get(take, weather.TemperatureMax, weather.TemperatureMin));

            return result;
        }
    }
}
