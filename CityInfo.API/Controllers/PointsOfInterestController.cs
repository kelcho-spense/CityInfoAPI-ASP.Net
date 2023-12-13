using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [Authorize]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    { 
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMapper mapper, IMailService mailService, ICityInfoRepository cityInfoRepository)
        {
             _logger = logger ?? throw new ArgumentException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mailService = mailService ?? throw new ArgumentException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
        }
    
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            // assume we need to allow only users from a certain city to view the POI of that city.
            var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;
            // A simple logic to ensure you can only view cities that match your city in
            if (!(await _cityInfoRepository.CityNameMatchesCityId(cityName, cityId)))
            {
                return Forbid();
            }

            // check if city exist first
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found.");
                return NotFound();
            }
            var pointsOfInterestForCity = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);
            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity));
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterestAsync(int cityId, int pointofinterestid)
        {
            //check if a city exist
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found.");
                return NotFound();
            }

            // check point of interest if the city is found

            var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointofinterestid);
            if (pointOfInterest == null)
            {
                return NotFound();
            } 
            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterestAsync(int cityId, PointOfInterestForCreationDto pointOfInterest)
        {
            // check if the city exist
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }

            var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest); // format obj to a form our entities can persist to our db.

            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);

            await _cityInfoRepository.SaveChangesAsync();

            // map back our obj to a dto so that we can return it.
            var createdPointOfInterestToReturn = _mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);

            //return a route where we can access our Obj from our db.
            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = createdPointOfInterestToReturn.Id
                }, createdPointOfInterestToReturn
             );
           
        }

        [HttpPut("{pointOfInterestId}")]

        public async Task<ActionResult> UpdatePointOfInterestAsync(int cityId, int pointOfInterestId, PointOfInterestForUpdateDto pointOfInterest)
        {
            //check if the city exist

            // check if the city exist
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }

            // find a point of interest
            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            //update
            _mapper.Map(pointOfInterest, pointOfInterestEntity);  // this will overide the values of the destination obj with the source obj

            await _cityInfoRepository.SaveChangesAsync(); // persist changes to the db via context.
            return NoContent();
        }

        [HttpPatch("{pointofinterestId}")]

        public async Task<ActionResult> PartiallyUpdatePointOfInterestAsync(int cityId, int pointofinterestId, 
            JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            //check if the city exist
            // check if the city exist
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }

            // find a point of interest
            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointofinterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }
            //create patch obj via mapping it to a dto
            var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

            //Apply patch
            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //validate pointOfInterestToPatch whether it contains all the valid fields

            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            //overide the current data of our entity obj with our valid patch obj
            _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync(); // persist changes to the db via context.

            return NoContent();
        }

        [HttpDelete("{pointofinterestId}")]

        public async Task<ActionResult> DeletePointOfInterestAsync(int cityId, int pointofinterestId)
        {
            //check if the city exist
            // check if the city exist
            if (!await _cityInfoRepository.CityExistAsync(cityId))
            {
                return NotFound();
            }

            // find a point of interest
            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointofinterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);

            await _cityInfoRepository.SaveChangesAsync(); // persist changes to the db via context.

            _mailService.Send(
                "Point of interest deleted.",
                $" Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted."
                );

            return NoContent();

        }
    }
}
