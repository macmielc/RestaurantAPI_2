
using RestaurantAPI_2.Entities;

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
            #region Rejestracja serwisów
            // 
            services.AddTransient<IWeatherForcastService, WeatherForcastService>();
            services.AddControllers();
            // Resjstrowania kontekstu bazy danych
            services.AddDbContext<RestaurantDBContext>();
            // Rejestracja Seedera
            services.AddScoped<RestaurantSeeder>();
            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RestaurantSeeder seeder) //, RestaurantSeeder seeder)
        {
            // Proces seedowania - wstrzykujemy serwis seedujacy RestaurantSeeder
            seeder.Seed();  

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
