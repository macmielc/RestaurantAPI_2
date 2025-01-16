
using RestaurantAPI_2.Entities;
using RestaurantAPI_2.Middlewere;
using RestaurantAPI_2.Services;

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
            
            services.AddControllers();
            // Resjstrowania kontekstu bazy danych
            services.AddDbContext<RestaurantDBContext>();
            // Rejestracja Seedera
            services.AddScoped<RestaurantSeeder>();
            // Metoda rozszerzająca (z namespace AutoMapper) do której musimy przekazać assembly w którym AutoMapper przeszuka wszystkie typy aby móc je rzutować.
            services.AddAutoMapper(this.GetType().Assembly);
            services.AddScoped<IRestaurantService, RestaurantService>();
            services.AddScoped<ErrorHandlingMiddleware>();
            services.AddScoped<RequestTimeMiddleware>();
            services.AddSwaggerGen(); // Dodawanie swaggera
            #endregion
        }
        /// <summary>
        /// Jak ma przebiegać zapytanie
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="seeder"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RestaurantSeeder seeder) //, RestaurantSeeder seeder)
        {
            // Proces seedowania - wstrzykujemy serwis seedujacy RestaurantSeeder
            seeder.Seed();  

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Wstawienie Middlewarea na koniec spowodowało by że instrukcja try cacth była by pomijan w kodzie a błędy nie były by przechwytywane 
            // dlatego należy zadbać o odpowiednią koilejność
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseMiddleware<RequestTimeMiddleware>();

            app.UseHttpsRedirection(); // Jeżeli klient wysle zapytanie na http zostanie przekirowane na https
            app.UseSwagger(); //Metoda odpowiedzialna za stworzenie pliku json na potrzeby swaggera
            app.UseSwaggerUI(c =>       // Deklrowanie loklizacji dokumnetacji
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "RestaurantAPI_2");
                
            });

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
