using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI_2.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
            var connection = _dbContext.Database.GetDbConnection();
            Console.WriteLine($"[SEED] Env={env.EnvironmentName}, Server={connection.DataSource}, Database={connection.Database}");

            if (!_dbContext.Database.CanConnect())
            {
                Console.WriteLine("[SEED] CanConnect=false. Pomijam migrację i seed.");
                return;
            }

            var pending = _dbContext.Database.GetPendingMigrations().ToList();
            Console.WriteLine($"[SEED] Pending migrations: {pending.Count}");

            _dbContext.Database.Migrate();
            Console.WriteLine("[SEED] Migrate OK.");

            if (!_dbContext.Roles.Any())
            {
                _dbContext.Roles.AddRange(GetRole());
                _dbContext.SaveChanges();
            }

            if (!_dbContext.Restaurants.Any())
            {
                _dbContext.Restaurants.AddRange(GetRestaurants());
                _dbContext.SaveChanges();
            }

            if (!_dbContext.Users.Any())
            {
                _dbContext.Users.AddRange(GetUsers());
                _dbContext.SaveChanges();
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
