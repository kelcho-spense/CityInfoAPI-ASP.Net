using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts
{
    public class CityInfoContext : DbContext
    {
        // this is how we set our entities to our context.
        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<PointOfInterest> PointsOfInterest { get; set; } = null!;

        //exposing our constructor to the dbcontextoptions
        public CityInfoContext(DbContextOptions<CityInfoContext> options) : base(options) {  }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().HasData(
                new City("New york City")
                {
                    Id = 1,
                    Description = "The one with the big pack."
                },
                new City("Nyeri")
                {
                    Id = 2,
                    Description = "The cold one."
                },
                new City("Karatina")
                {
                    Id = 3,
                    Description = "The one with the big market."
                });

            modelBuilder.Entity<PointOfInterest>().HasData(
                new PointOfInterest("Central Park")
                {
                    Id = 1,
                    CityId = 1,
                    Description = "The biggest pack in the city."
                },
                new PointOfInterest("Empire state building")
                {
                    Id = 2,
                    CityId = 2,
                    Description = "The biggest building in the city."
                },
                new PointOfInterest("Cathedral")
                {
                    Id = 3,
                    CityId = 2,
                    Description = "The biggest church in the city."
                },
                 new PointOfInterest("Train station")
                 {
                     Id = 4,
                     CityId = 3,
                     Description = "The biggest station in the city."
                 });

            base.OnModelCreating(modelBuilder);
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlite("connectionstring");
        //    base.OnConfiguring(optionsBuilder);
        //}

    }
}
