using AutoMapper;
using CityInfo.io.Models;
using CityInfo.io.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace CityInfo.io.Controllers
{

    [Route("api/v{version:apiVersion}/cities/{cityId}/PointsOfInterest")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]

    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> logger;
        private readonly IMailService mailService;
        private readonly ICityInfoRepository cityInfoRepository;
        private readonly IMapper mapper;

        //private readonly CitiesDataStore _citiesDataStore;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger 
            , IMailService mailService , CitiesDataStore citiesDataStore,
            ICityInfoRepository cityInfoRepository , IMapper mapper)
        {
            if (mailService is null)
            {
                throw new ArgumentNullException(nameof(mailService));
            }

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mailService = mailService;
            this.cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            //_citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
        }

        [HttpGet]
        public async Task< ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            if(!await cityInfoRepository.CityExistAsync(cityId)) 
            {
                logger.LogInformation($"City with id {cityId} wasn't found when assessing points of interest.");

                return NotFound();
            }
            var pointOfInterestForCity = cityInfoRepository.GetAllPointsOfInterestForCityAsync(cityId);
            return Ok(mapper.Map<IEnumerable< PointOfInterestDto>>(pointOfInterestForCity));

            //try
            //{
            //    throw new Exception("sample exception !");
            //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            //    if (city == null)
            //    {
            //        logger.LogInformation($"City with id {cityId} wasn't found when assessing points of interest.");
            //        return NotFound();
            //    }

            //    return Ok(city.PointsOfInterest);
            //}
           
            

            // catch (Exception ex)
            //{
            //    logger.LogCritical($"Exception white getting points of interest for city with id {cityId}", ex);
            //    return StatusCode(500, "A problem happened white handling your request.");


            //}
        }
        [HttpGet("{pointOfInterestId}",Name = "GetPointOfInterest")]
        public async Task< ActionResult<IEnumerable<PointOfInterestDto>>> GetPointOfInterest(int cityId , int pointOfInterestId)
        {
            //var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
            //if (city == null) { return NotFound(); }
            //var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
            //if(pointOfInterest == null) { return NotFound(); }
            //return Ok(pointOfInterest);

            if(!await cityInfoRepository.CityExistAsync (cityId))
            {
                return NotFound();
            }
            var pointOfInterest = cityInfoRepository.GetPointOfInterestForCityAsync (cityId, pointOfInterestId);
            if(pointOfInterest == null) { return  NotFound(); }
            return Ok(mapper.Map<PointOfInterestDto> (pointOfInterest));
        }
        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId,
            PointOfInterestForCreationDto pointOfInterest)
        {
            if (!await cityInfoRepository.CityExistAsync(cityId)) { return NotFound(); };
            var finalPointOfInterest = mapper.Map<Entities.PointOfInterest>(pointOfInterest);
            await cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);

            await cityInfoRepository.SaveChangesAsync();

            var CreatedPointOfInterestForReturn = mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);
           
            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = CreatedPointOfInterestForReturn.Id
                }, CreatedPointOfInterestForReturn);
        }
        [HttpPut("{PointOfInterestId}")]
        public async Task< ActionResult<PointOfInterestDto>> UpdatePointOfInterest(int cityId, int PointOfInterestId,
            PointOfInterestForUpdateDto pointOfInterest)

        {
            if(! await cityInfoRepository.CityExistAsync (cityId)) { return NotFound(); };

            var pointOfInterestEntity = await cityInfoRepository.GetPointOfInterestForCityAsync(cityId, PointOfInterestId);
            if (pointOfInterestEntity == null) {  return NotFound(); };

            // the values of the pointOfInterestEntity will be overriden by the values of the pointOfInterest
            // that is a special case of the auto mapper
            mapper.Map(pointOfInterest, pointOfInterestEntity);

            await cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }
        [HttpPatch("{PointOfInterestId}")]
        public async Task< ActionResult<PointOfInterestDto> >partiallyUpdatePointOfInterest(int cityId, int PointOfInterestId
            , JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            if (!await cityInfoRepository.CityExistAsync(cityId)) { return NotFound(); };

            var pointOfInterestEntity = await cityInfoRepository.GetPointOfInterestForCityAsync(cityId, PointOfInterestId);
            if (pointOfInterestEntity == null) { return NotFound(); };

            var pointOfInterestToPatch = mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);
            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            // this validation will make the data annotation for the properties work well 

            if (!TryValidateModel(pointOfInterestToPatch)) { return BadRequest(ModelState); }

           mapper.Map(pointOfInterestEntity,pointOfInterestToPatch);
            await cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{PointOfInterestId}")]
        public async Task< ActionResult<PointOfInterestDto>> DeletePointOfInterest(int cityId, int PointOfInterestId)
        {
            if (!await cityInfoRepository.CityExistAsync(cityId)) { return NotFound(); };

            var pointOfInterestEntity = await cityInfoRepository.GetPointOfInterestForCityAsync(cityId, PointOfInterestId);
            if (pointOfInterestEntity == null) { return NotFound(); };

            cityInfoRepository.DeletePointOfInterestForCity(pointOfInterestEntity);
            await cityInfoRepository.SaveChangesAsync();
            //city.PointsOfInterest.Remove(PointOfInterestFromStore);
            mailService.send("Point of interest deleted.",
                $"Point of interest {pointOfInterestEntity.Name} with an id {pointOfInterestEntity.Id} was deleted");
            return NoContent();
        }

    }
}
