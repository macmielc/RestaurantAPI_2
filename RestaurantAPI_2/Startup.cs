
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI_2.Authorization;
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

            // rejestrowania customowej polityki autoryzacji
            services.AddAuthorization(options =>
                {
                    options.AddPolicy("HasNationality", builder =>
                        builder.RequireClaim("Nationality", new string[] {"Poland", "Dutch"})); // prywatna polityka z wartościami które musi spełniać
                    options.AddPolicy("AtLeast20", builder => builder.AddRequirements(new MinimumAgeRequirement(20)));
                    options.AddPolicy("MinRestaurantNr", builder => builder.AddRequirements(new RestaurantNumberRequirement(2)));
                });
            #endregion
            // Na jedno zapyatnie jedną instancję
            //services.AddScoped<IStartup, Startup>(); 
            #region Rejestracja serwisów
            // 
            services.AddScoped<IAuthorizationHandler, RestaurantNumberRequirementHandler>(); // rozdział 47
            services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, ResourcesOperationRequirementsHandler>(); // nie tworzymy prywatnej polityki
            
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
            services.AddScoped<IValidator<RestaurantQuery>, RestaurantQueryValidator>();
            services.AddScoped<RequestTimeMiddleware>();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddHttpContextAccessor();  // Dzieki temu możemy wstrzyknąć od UserContextService IHttpContextAccessor
            services.AddSwaggerGen();           // Dodawanie swaggera
            #region CORS
            services.AddCors(options =>
            {
                options.AddPolicy("FrontEndClient", builder =>
                    builder.AllowAnyMethod()    // dopuszczenie jakiejkolwiek metody http (get, post, put, delete itd.)
                        .AllowAnyHeader()       // dopuszczenie jakiegokolwiek nagłówka http
                                                // Mozna (nalezy) przenieść dopuszczalne domeny do pliku  appsetting.json
                        .WithOrigins(configRoot["AllowedOrigines"])); //, "http://localhost:8080/" })); // AllowAnyOrigin - pozwala na dostęp z każdego adresu, WithOrigins - pozwala tylko z określonego adresu
                        //.AllowAnyOrigin()           // dopuszczenie jakiegokolwiek adresu (nie jest zalecane w produkcji)
            });
            #endregion

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
            app.UseStaticFiles(); // Dodanie obsługi plików statycznych (np. zdjęć) - domyślnie szuka katalogu wwwroot
            // Uruchomienie polityki CORS
            app.UseCors("FrontEndClient"); // Nazwa polityki którą chcemy uruchomić

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
            app.UseSwaggerUI(c =>       // Deklrowanie loklizacji dokumnetacji oraz konfiguracja swaggera http://localhost:5101/swagger/index.html
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
