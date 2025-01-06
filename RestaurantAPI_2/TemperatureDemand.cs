namespace RestaurantAPI_2
{
    public class TemperatureDemand
    {
        public int TemperatureMax { get; set; }

        public int TemperatureMin { get; set; }

        internal bool ValidateTemperature { get {  return TemperatureMax > TemperatureMin; } }
    }
}
