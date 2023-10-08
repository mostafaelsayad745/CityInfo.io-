using CityInfo.io.Entities;

namespace CityInfo.io.Services
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync();
        Task<(IEnumerable<City>, PaginationMetadata) > GetCitiesAsync(string? name,string? searchQuery,int pageNumber,int pageSize);
        Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest);
        Task<bool>  CityExistAsync(int cityId);
        Task<IEnumerable<PointOfInterest>> GetAllPointsOfInterestForCityAsync(int cityId);
        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId);
        Task AddPointOfInterestForCityAsync (int cityId , PointOfInterest pointOfInterest); // and add is not i/o operation but here we get a city form the database
        void DeletePointOfInterestForCity (PointOfInterest pointOfInterest); // because removing is not i/o operation
        Task<bool> SaveChangesAsync();
    }
}
