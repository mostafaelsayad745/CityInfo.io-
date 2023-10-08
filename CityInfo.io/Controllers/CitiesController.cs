using AutoMapper;
using CityInfo.io.Models;
using CityInfo.io.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CityInfo.io.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/cities")]
    [ApiVersion("1.0")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository cityInfoRepository;
        private readonly IMapper _mapper;
        const int MaxPageSizeForCities = 20;

        public CitiesController(ICityInfoRepository cityInfoRepository , IMapper mapper)
        {
            this.cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            
        }

        
        [HttpGet]
        public async Task< ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>>
            GetCities(string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10) 
        {
            if (pageSize > MaxPageSizeForCities)
            {
                pageSize = MaxPageSizeForCities;
            }

            var (cityEntities, PaginationMetadata) = await cityInfoRepository.GetCitiesAsync(name,searchQuery,pageNumber,pageSize);

            //var results = new List<CityWithoutPointsOfInterestDto>();
            //foreach (var cityEntity in cityEntities)
            //{
            //    results.Add(new CityWithoutPointsOfInterestDto()
            //    {
            //        Id = cityEntity.Id,
            //        Name = cityEntity.Name,
            //        Description = cityEntity.Description,
            //    });
            //}
            //Response.Headers.Add("X-Pagination",
            //   JsonSerializer.Serialize(PaginationMetadata));
            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));
                
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(
           int id, bool includePointsOfInterest = true)
        {
            var city = await cityInfoRepository.GetCityAsync(id, includePointsOfInterest);
            if (city == null)
            {
                return NotFound();
            }

            if (includePointsOfInterest)
            {
                return Ok(_mapper.Map<CityDto>(city));
            }

            return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city));
        }
    }
}
