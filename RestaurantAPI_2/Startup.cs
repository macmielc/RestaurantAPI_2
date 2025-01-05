
namespace RestaurantAPI_2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            configRoot = configuration;
        }

        public IConfiguration configRoot { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Na jedno zapyatnie jedną instancję
            //services.AddScoped<IStartup, Startup>(); 
            // 
            services.AddTransient<IWeatherForcastService, WeatherForcastService>();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) //, RestaurantSeeder seeder)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection(); // Jeżeli klient wysle zapytanie na http zostanie przekirowane na https
            
            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
