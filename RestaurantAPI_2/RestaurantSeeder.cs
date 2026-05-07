using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI_2.Entities;

namespace RestaurantAPI_2
{
    public class RestaurantSeeder
    {
        private readonly RestaurantDBContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;

        public RestaurantSeeder(RestaurantDBContext dbContext, IPasswordHasher<User> passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }
        /// <summary>
        /// Metoda dodająca dane do tabel na bazie danych
        /// </summary>
        public void Seed(IWebHostEnvironment env)
        {
            try
            {
                if (_dbContext.Database.CanConnect())
                {
                    // AUTOMATYCZNA MIGRACJA BAZY DANYCH
                    var pendingMigrations = _dbContext.Database.GetPendingMigrations();

                    // Sprawdzenie czy istnieją jakieś niezaaplikowane migracje
                    if (pendingMigrations != null && pendingMigrations.Any())
                    {
                        try
                        {
                            Console.WriteLine($"=== Rozpoczynam migrację bazy danych ({(env.IsDevelopment()? "Development local"  :  env.IsProduction() ? "Production - Azure SQL" : "Unknown")}) ===");
                            _dbContext.Database.Migrate(); // Zastosowanie wszystkich niezaaplikowanych migracji
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"!!! BŁĄD MIGRACJI BAZY DANYCH: {ex.Message}");
                            Console.WriteLine($"!!! Stack Trace: {ex.StackTrace}");
                        }
                        finally
                        {
                            Console.WriteLine($"=== Migracja bazy danych zakończona pomyślnie ({(env.IsDevelopment()? "Development local"  :  env.IsProduction() ? "Production - Azure SQL" : "Unknown")}) ===");
                        }
                    }

                    if (!_dbContext.Roles.Any())
                    {
                        var roles = GetRole();
                        _dbContext.Roles.AddRange(roles);

                        // Zapisywanie zmian na kontekscie bazy danych
                        _dbContext.SaveChanges();
                    }


                    if (!_dbContext.Restaurants.Any())
                    {
                        var restaurants = GetRestaurants();
                        _dbContext.Restaurants.AddRange(restaurants);

                        // Zapisywanie zmian na kontekscie bazy danych
                        _dbContext.SaveChanges();
                    }

                    if (!_dbContext.Users.Any())
                    {
                        var users = GetUsers();
                        _dbContext.Users.AddRange(users);

                        // Zapisywanie zmian na kontekscie bazy danych
                        _dbContext.SaveChanges();
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"!!! BŁĄD SEEDOWANIA: {ex.Message}");
            }
        }

        private IEnumerable<User> GetUsers()
        {
            var users = new List<User>()
            {
                new User() // #1
                {
                    Email = "jan.kowalski@example.com",
                    FirstName = "Jan",
                    LastName = "Kowalski",
                    DateOfBirth = new DateTime(1979, 10, 9),
                    Nationality = "Polish",
                    RoleId = 2
                },
                new User() // #2
                {
                    Email = "marta.banyk@example.com",
                    FirstName = "Marta",
                    LastName = "Banyk",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Nationality = "Polish",
                    RoleId = 1
                },
                new User() // #3
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
                new Restaurant() // #1
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
                    Address =  new Address()
                    {
                        City = "Kraków",
                        Street = "Długa 5",
                        PostalCode = "30-301"
                    }
                },
                new Restaurant() // #2
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
                    Address =  new Address()
                    {
                        City = "Kraków",
                        Street = "Szewska 2",
                        PostalCode = "30-001"
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
            };

            return roles;
        }
    }
}
