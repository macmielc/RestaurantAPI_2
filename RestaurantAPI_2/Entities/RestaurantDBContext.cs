using Microsoft.EntityFrameworkCore;

namespace RestaurantAPI_2.Entities
{
    public class RestaurantDBContext : DbContext
    {
        // Usuniete w dniu 20260502 - refkatoring - przeniesienie do appsettings.json na potrzeby Azure
        //private string _conectionstring = "Server=(localdb)\\mssqllocaldb;Database=RestaurantDb2;Trusted_Connection=True;";

        public RestaurantDBContext(DbContextOptions<RestaurantDBContext> options) : base(options)
        {
            
        }

        public DbSet<Restaurant> Restaurants { get; set; }
        
        public DbSet<Address> Address { get; set; } 

        public DbSet<Dish> Dishes { get; set; }

        public DbSet<Role > Roles { get; set; }

        public DbSet<User> Users { get; set; }  
        /// <summary>
        /// Tworzenie bazy danych konfiguracja kolumn tabel
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Property(r => r.Email).IsRequired();
            //modelBuilder.Entity<User>().Property(r => r.FirstName).IsRequired(false);
            //modelBuilder.Entity<User>().Property(r => r.LastName).IsRequired(false);
            //modelBuilder.Entity<User>().Property(r => r.Name).IsRequired(false);

            modelBuilder.Entity<Role>().Property(r => r.Name).IsRequired(true);

            modelBuilder.Entity<Restaurant>().Property(r => r.Name).IsRequired(false).HasMaxLength(25);
            modelBuilder.Entity<Restaurant>().Property(r => r.Description).IsRequired(false);
            modelBuilder.Entity<Restaurant>().Property(r => r.Category).IsRequired(false);
            modelBuilder.Entity<Restaurant>().Property(r => r.ContactNumber).IsRequired(false);
            modelBuilder.Entity<Restaurant>().Property(r => r.ContactEmail).IsRequired(false);
            //modelBuilder.Entity<Restaurant>().Property(r => r.Dishes).IsRequired(false);

            modelBuilder.Entity<Dish>().Property(d => d.Name).IsRequired(false);
            modelBuilder.Entity<Dish>().Property(d => d.Description).IsRequired(false);

            modelBuilder.Entity<Address>().Property(a => a.City).IsRequired(false).HasMaxLength(50);
            modelBuilder.Entity<Address>().Property(a => a.Street).IsRequired(false).HasMaxLength(50);
            modelBuilder.Entity<Address>().Property(a => a.PostalCode).IsRequired(false);
        

        }
        // Usuniete w dniu 20260502 - refkatoring - przeniesienie do appsettings.json na potrzeby Azure
        ///// <summary>
        ///// Tworzenie połączenia z bazą danych
        ///// </summary>
        ///// <param name="optionsBuilder"></param>
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(_conectionstring);
        //   // base.OnConfiguring(optionsBuilder);
        //}

    }
}
