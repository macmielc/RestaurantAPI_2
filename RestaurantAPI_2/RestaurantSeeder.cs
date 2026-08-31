using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI_2.Entities;
using System.Data.Common;

namespace RestaurantAPI_2
{
    public class RestaurantSeeder
    {
        private readonly RestaurantDBContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<RestaurantSeeder> _logger;

        public RestaurantSeeder(
            RestaurantDBContext dbContext,
            IPasswordHasher<User> passwordHasher,
            ILogger<RestaurantSeeder> logger)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        /// <summary>
        /// Metoda dodająca dane do tabel na bazie danych
        /// </summary>
        public void Seed(IWebHostEnvironment env)
        {
            var connection = _dbContext.Database.GetDbConnection();

            _logger.LogInformation(
                "[SEED] Start. Env={Env}, Server={Server}, Database={Database}, ConnectionType={Type}",
                env.EnvironmentName,
                connection.DataSource,
                connection.Database,
                connection.GetType().FullName);

            try
            {
                if (!_dbContext.Database.CanConnect())
                {
                    _logger.LogError(
                        "[SEED] CanConnect=false. Brak połączenia z bazą. Server={Server}, Database={Database}",
                        connection.DataSource,
                        connection.Database);

                    return;
                }

                var pendingMigrations = _dbContext.Database.GetPendingMigrations().ToList();

                _logger.LogInformation(
                    "[SEED] Pending migrations count={Count}. Names={Names}",
                    pendingMigrations.Count,
                    pendingMigrations.Any() ? string.Join(", ", pendingMigrations) : "<none>");


                _dbContext.Database.Migrate();
                _logger.LogInformation("[SEED] Migrate() zakończone.");

                if (!_dbContext.Roles.Any())
                {
                    _dbContext.Roles.AddRange(GetRole());
                    _dbContext.SaveChanges();
                    _logger.LogInformation("[SEED] Dodano dane domyślne do tabeli Roles.");
                }

                if (!_dbContext.Restaurants.Any())
                {
                    _dbContext.Restaurants.AddRange(GetRestaurants());
                    _dbContext.SaveChanges();
                    _logger.LogInformation("[SEED] Dodano dane domyślne do tabeli Restaurants.");
                }

                if (!_dbContext.Users.Any())
                {
                    _dbContext.Users.AddRange(GetUsers());
                    _dbContext.SaveChanges();
                    _logger.LogInformation("[SEED] Dodano dane domyślne do tabeli Users.");
                }

                _logger.LogInformation("[SEED] Koniec.");
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex,
                    "[SEED][SQL] Number={Number}, State={State}, Class={Class}, Procedure={Procedure}, LineNumber={LineNumber}, Server={Server}, ConnectionId={ConnectionId}",
                    ex.Number,
                    ex.State,
                    ex.Class,
                    ex.Procedure,
                    ex.LineNumber,
                    ex.Server,
                    ex.ClientConnectionId);

                throw;
            }
            catch (DbException ex)
            {
                _logger.LogError(ex,
                    "[SEED][DB] Type={Type}, ErrorCode={ErrorCode}, Message={Message}",
                    ex.GetType().FullName,
                    ex.ErrorCode,
                    ex.Message);

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SEED] Nieoczekiwany błąd seedowania.");
                throw;
            }
        }

        private IEnumerable<User> GetUsers()
        {
            var users = new List<User>()
            {
                new User()
                {
                    Email = "jan.kowalski@example.com",
                    FirstName = "Jan",
                    LastName = "Kowalski",
                    DateOfBirth = new DateTime(1979, 10, 9),
                    Nationality = "Polish",
                    RoleId = 2
                },
                new User()
                {
                    Email = "marta.banyk@example.com",
                    FirstName = "Marta",
                    LastName = "Banyk",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Nationality = "Polish",
                    RoleId = 1
                },
                new User()
                {
                    Email = "Balbina.mielcarek@example.com",
                    FirstName = "Balbina",
                    LastName = "Mielcarek",
                    DateOfBirth = new DateTime(1992, 12, 27),
                    Nationality = "Polish",
                    RoleId = 3
                },
            };

            foreach (var user in users)
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, "balbiLubiSzczekac3");
            }

            return users;
        }

        private IEnumerable<Restaurant> GetRestaurants()
        {
            var resturants = new List<Restaurant>()
            {
                new Restaurant()
                {
                    Name = "KFC",
                    Category = "Fast Food",
                    Description = "KFC (short for Knetucky Fried Chicken is an American fast food restaurant chain headquartered in ...",
                    ContactEmail = "contact@kfc.com",
                    HasDelivery = true,
                    Dishes = new List<Dish>()
                    {
                        new Dish()
                        {
                            Name = "Nashville Hot Chicken",
                            Price = 10.30m
                        },
                        new Dish()
                        {
                            Name = "Chicken Nuget",
                            Price = 10.30m
                        },
                    },
                    Address = new Address()
                    {
                        City = "Kraków",
                        Street = "Długa 5",
                        PostalCode = "30-301"
                    }
                },
                new Restaurant()
                {
                    Name = "McDonald",
                    Category = "Fast Food",
                    Description = "McDonald's Coropration (McDonald's), incorporated on December 21, 1964, operates and franchises ... ",
                    ContactEmail = "contact@mcdonald.com",
                    HasDelivery = true,
                    Dishes = new List<Dish>()
                    {
                        new Dish()
                        {
                            Name = "Big Mac",
                            Price = 9.30m
                        },
                        new Dish()
                        {
                            Name = "Burger",
                            Price = 5.30m
                        },
                    },
                    Address = new Address()
                    {
                        City = "Kraków",
                        Street = "Szewska 2",
                        PostalCode = "30-001"
                    }
                },
                new Restaurant()
                {
                    Name = "Szczaw i Mirabelki",
                    Category = "Kuchnia wegetariańska",
                    Description = "McDonald's Coropration (McDonald's), incorporated on December 21, 1964, operates and franchises ... ",
                    ContactEmail = "szaw&mirabelki@szczaw.com",
                    HasDelivery = true,
                    Dishes = new List<Dish>()
                    {
                        new Dish()
                        {
                            Name = "Sushi wege z avokado",
                            Price = 39.30m
                        },
                        new Dish()
                        {
                            Name = "Sushi wege z mango",
                            Price = 35.30m
                        },
                        new Dish()
                        {
                            Name = "Wege stripsy",
                            Price = 28.00m
                        },
                    },
                    Address = new Address()
                    {
                        City = "Poznań",
                        Street = "Wojsciechowskiego 40",
                        PostalCode = "60-681"
                    }
                },
            };

            return resturants;
        }

        private IEnumerable<Role> GetRole()
        {
            var roles = new List<Role>()
            {
                new Role()
                {
                    Name = "Admin"
                },
                new Role()
                {
                    Name = "User"
                },
                new Role()
                {
                    Name = "Manager"
                },
                new Role()
                {
                    Name = "Viwer"
                },
            };

            return roles;
        }
    }
}
