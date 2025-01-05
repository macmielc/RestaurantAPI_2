namespace RestaurantAPI_2
{
    public interface IWeatherForcastService
    {
        IEnumerable<WeatherForecast> Get();
    }
}