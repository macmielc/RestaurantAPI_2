using Microsoft.EntityFrameworkCore;
using RestaurantAPI_2.Entities;

namespace RestaurantAPI_2
{
    public class RestaurantSeeder
    {
        private readonly RestaurantDBContext _dbContext;

        public RestaurantSeeder(RestaurantDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        /// <summary>
        /// Metoda dodająca dane do tabel na bazie danych
        /// </summary>
        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {

                var pendingMigrations = _dbContext.Database.GetPendingMigrations();

                // Sprawdzenie czy istnieją jakieś niezaaplikowane migracje
                if (pendingMigrations != null && pendingMigrations.Any())
                {
                    _dbContext.Database.Migrate(); // Zastosowanie wszystkich niezaaplikowanych migracji
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

                
            }
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
