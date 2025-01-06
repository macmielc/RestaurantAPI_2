namespace RestaurantAPI_2
{
    public interface IWeatherForcastService
    {
        IEnumerable<WeatherForecast> Get();

        IEnumerable<WeatherForecast> Get(int take, int max, int min);
    }
}