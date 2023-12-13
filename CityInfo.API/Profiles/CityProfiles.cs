using AutoMapper;

namespace CityInfo.API.Profiles
{
    public class CityProfiles : Profile
    {
        public CityProfiles()
        {
            CreateMap<Entities.City, Models.CityWithoutPointsOfInterestDto>();  //will map properties in City entities to CityWithoutPointsOfInterestDto & ignore null refeernces
            CreateMap<Entities.City, Models.CityDto>(); //will map properties in City entities to CityDto

        }
    }
}
