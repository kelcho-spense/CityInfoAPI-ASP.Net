using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")] // we define once the route attribute for all routes in this controller
    [Authorize]     
    [ApiController] // api controller attribute -> adds behaviours of a controller
    public class CitesControllers : ControllerBase   //ControllerBase has basic fuctionalities of a controller
    {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        const int maxCitiesPageSize = 20;

        public CitesControllers(ICityInfoRepository cityInfoRepository, IMapper mapper  ) 
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities ( [FromQuery] string? name, 
            string? searchQuery, int pageNumber = 1, int pageSize = 10) 
        {
            if(pageSize > maxCitiesPageSize)
            {
                pageSize = maxCitiesPageSize;
            }

           var (cityEntities, paginationMetadata) = await _cityInfoRepository.GetCitiesAsync(name, searchQuery,pageNumber,pageSize); // get all cities & paginationMetadata
            
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
            
            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities)); 
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity (int id, bool includePointsOfInterest = false)
        {
            //return cities with point of interests and their count.
            var city = await  _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);
            if(city == null)
            {
                return NotFound();
            } 

            //return city with points of interest of a city without
            if(includePointsOfInterest)
            {
                return Ok(_mapper.Map<CityDto>(city)); // return obj of type cityDTO
            }
            return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city)); // return obj of type CityWithoutPointsOfInterestDto

        }
    }
}
