using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            configRoot = configuration;
            _environment = environment;
        }

        private readonly IWebHostEnvironment _environment;

        public IConfiguration configRoot { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            #region Konfiguracja Jwt
            var authenticationSettings = new AuthenticationSettings();

            configRoot.GetSection("Authentication").Bind(authenticationSettings);

            if (string.IsNullOrWhiteSpace(authenticationSettings.JwtKey))
            {
                throw new InvalidOperationException("Brak konfiguracji Authentication:JwtKey.");
            }

            if (string.IsNullOrWhiteSpace(authenticationSettings.JwtIssuer))
            {
                throw new InvalidOperationException("Brak konfiguracji Authentication:JwtIssuer.");
            }

            if (authenticationSettings.JwtExpireDays <= 0)
            {
                throw new InvalidOperationException("Authentication:JwtExpireDays musi być > 0.");
            }

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

            // DODANIE: Konfiguracja Response Caching
            services.AddResponseCaching(options =>
            {
                options.MaximumBodySize = 1024; // Maksymalny rozmiar cache'owanej odpowiedzi w bajtach
                options.UseCaseSensitivePaths = true; // Uwzględnianie wielkości liter w ścieżkach
            });

            // Rejestracja kontekstu bazy danych z automatyczną migracją // Poprawione 20260502
            services.AddDbContext<RestaurantDBContext>(
                options => options.UseSqlServer(_environment.IsDevelopment() ? 
                configRoot.GetConnectionString("DevelopmentConnectionDB") : 
                configRoot.GetConnectionString("RestaurantDBConnection")));
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
            #endregion
            #region CORS
            services.AddCors(options =>
            {
                options.AddPolicy("FrontEndClient", builder =>
                    builder.AllowAnyMethod()    // dopuszczenie jakiejkolwiek metody http (get, post, put, delete itd.)
                        .AllowAnyHeader()       // dopuszczenie jakiegokolwiek nagłówka http
                                                // Mozna (nalezy) przenieść dopuszczalne domeny do pliku  appsetting.json
                        .WithOrigins(configRoot["AllowedOrigins"] ?? "http://localhost:8080")); //, "http://localhost:8080/" })); // AllowAnyOrigin - pozwala na dostęp z każdego adresu, WithOrigins - pozwala tylko z określonego adresu
                        //.AllowAnyOrigin()           // dopuszczenie jakiegokolwiek adresu (nie jest zalecane w produkcji)
            });
            #endregion
            
        }
        /// <summary>
        /// Jak ma przebiegać zapytanie
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="seeder"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RestaurantSeeder seeder, RestaurantDBContext dbContext) //, RestaurantSeeder seeder)
        {
            app.UseResponseCaching(); // Dodanie obsługi cache'owania odpowiedzi
            app.UseStaticFiles(); // Dodanie obsługi plików statycznych (np. zdjęć) - domyślnie szuka katalogu wwwroot
            // Uruchomienie polityki CORS
            app.UseCors("FrontEndClient"); // Nazwa polityki którą chcemy uruchomić

            // Proces seedowania - wstrzykujemy serwis seedujacy RestaurantSeeder
            seeder.Seed(env);  

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
            // Metoda odpowiedzialna za stworzenie pliku json na potrzeby swaggera
            app.UseSwagger();
            
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
