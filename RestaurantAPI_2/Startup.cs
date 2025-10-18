
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI_2.Entities;
using RestaurantAPI_2.Middlewere;
using RestaurantAPI_2.Models;
using RestaurantAPI_2.Models.Validators;
using RestaurantAPI_2.Services;
using System.Text;

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
            #region Konfiguracja Jwt
            var authenticationSettings = new AuthenticationSettings();

            configRoot.GetSection("Authentication").Bind(authenticationSettings);

            services.AddSingleton(authenticationSettings);

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = "Bearer";
                option.DefaultScheme = "Bearer";
                option.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(cfg =>
                { 
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = authenticationSettings.JwtIssuer,
                        ValidAudience = authenticationSettings.JwtIssuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))

                    };
                });
            #endregion
            // Na jedno zapyatnie jedną instancję
            //services.AddScoped<IStartup, Startup>(); 
            #region Rejestracja serwisów
            // 

            services.AddControllers();// - https://github.com/FluentValidation/FluentValidation/issues/1965 nie powinno być używane .AddFluentValidation(); 
            //Dodawanie Fluent Walidatora.AddFluentValidation();
            services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
            // Resjstrowania kontekstu bazy danych
            services.AddDbContext<RestaurantDBContext>();
            // Rejestracja Seedera
            services.AddScoped<RestaurantSeeder>();
            // Metoda rozszerzająca (z namespace AutoMapper) do której musimy przekazać assembly w którym AutoMapper przeszuka wszystkie typy aby móc je rzutować.
            services.AddAutoMapper(this.GetType().Assembly);
            services.AddScoped<IAccountService,  AccountService>();
            services.AddScoped<IRestaurantService, RestaurantService>();
            services.AddScoped<IRestaurantService, RestaurantService>();
            services.AddScoped<IDishService, DishService>();
            services.AddScoped<ErrorHandlingMiddleware>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
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
            app.UseAuthentication();

            app.UseHttpsRedirection();  // Jeżeli klient wysle zapytanie na http zostanie przekirowane na https
            app.UseSwagger();           // Metoda odpowiedzialna za stworzenie pliku json na potrzeby swaggera
            app.UseSwaggerUI(c =>       // Deklrowanie loklizacji dokumnetacji oraz konfiguracja swaggera
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "RestaurantAPI_2");              
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
