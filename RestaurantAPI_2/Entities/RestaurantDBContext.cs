using Microsoft.EntityFrameworkCore;

namespace RestaurantAPI_2.Entities
{
    public class RestaurantDBContext : DbContext
    {
        private string _conectionstring = "Server=(localdb)\\mssqllocaldb;Database=RestaurantDb2;Trusted_Connection=True;";
        public DbSet<Restaurant> Restaurants { get; set; }
        
        public DbSet<Address> Address { get; set; } 

        public DbSet<Dish> Dishes { get; set; }
        /// <summary>
        /// Tworzenie bazy danych
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Restaurant>().Property(r => r.Name).IsRequired().HasMaxLength(25);
            modelBuilder.Entity<Dish>().Property(d => d.Name).IsRequired();

            //base.OnModelCreating(modelBuilder);
        }
        /// <summary>
        /// Tworzenie połąćzenia z bazą danych
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_conectionstring);
           // base.OnConfiguring(optionsBuilder);
        }

    }
}
