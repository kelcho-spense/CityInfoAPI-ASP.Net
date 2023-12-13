using CityInfo.API.Models;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        public List<CityDto> Cities { get; set; }
        //public static CitiesDataStore Current { get; } = new CitiesDataStore();
        public CitiesDataStore() { 
            Cities = new List<CityDto>()
            {
                new CityDto()
                {
                    Id = 1,
                    Name = "Kagio",
                    Description = "Best market place",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id= 1,
                            Name = "Food Market",
                            Description = "buy all type of fruits"
                        },
                        new PointOfInterestDto()
                        {
                            Id= 2,
                            Name = "Catholic Parish",
                            Description = "A big church structure"
                        },
                    }
                },
                 new CityDto()
                 {
                    Id = 2,
                    Name = "Kutus",
                    Description = "Houses the counties HQ",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id= 1,
                            Name = "WaterFall",
                            Description = "Nice schene"
                        },
                        new PointOfInterestDto()
                        {
                            Id= 2,
                            Name = "Dam",
                            Description = "A big water reservor"
                        },
                    }
                 },
                  new CityDto()
                  {
                        Id = 3,
                        Name = "Kerugoya",
                        Description = "Administrative town",
                         PointsOfInterest = new List<PointOfInterestDto>()
                        {
                            new PointOfInterestDto()
                            {
                                Id= 1,
                                Name = "Big Stadium",
                                Description = "Nice schene"
                            },
                            new PointOfInterestDto()
                            {
                                Id= 2,
                                Name = "Hospital",
                                Description = "A big level 5 hospital"
                            },
                        }
                   },
                   new CityDto()
                   {
                        Id = 4,
                        Name = "Kagumo",
                        Description = "Tea Farmers choice",
                        PointsOfInterest = new List<PointOfInterestDto>()
                        {
                            new PointOfInterestDto()
                            {
                                Id= 1,
                                Name = "Big Stadium",
                                Description = "Nice schene"
                            },
                            new PointOfInterestDto()
                            {
                                Id= 2,
                                Name = "Hospital",
                                Description = "A big level 5 hospital"
                            },
                        }
                   },
            };
        }

    }
}
