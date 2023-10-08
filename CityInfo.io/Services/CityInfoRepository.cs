﻿using CityInfo.io.DbContexts;
using CityInfo.io.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.AccessControl;

namespace CityInfo.io.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context) 
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                return await _context.Cities.Include(c => c.pointOfInterests)
                    .Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }

            return await _context.Cities
                  .Where(c => c.Id == cityId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest>> GetAllPointsOfInterestForCityAsync(int cityId)
        {
            return await _context.PointOfInterests
                           .Where(p => p.CityId == cityId).ToListAsync();
        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
        {
            return await _context.PointOfInterests
                           .Where(p => p.CityId == cityId && p.Id == pointOfInterestId)
                           .FirstOrDefaultAsync();
        }

        public async Task<bool> CityExistAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId,false);
            if(city != null)
            {
                city.pointOfInterests.Add(pointOfInterest);
            }
        }
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public void DeletePointOfInterestForCity(PointOfInterest pointOfInterest)
        {
            _context.PointOfInterests.Remove(pointOfInterest);
        }

        public async Task<(IEnumerable<City>,PaginationMetadata)> GetCitiesAsync(string? name , string? searchQuery 
            , int pageNumber , int pageSize )
        {
           
            var collection = _context.Cities as IQueryable<City>;
            if(!string.IsNullOrEmpty(name))
            {
                name = name.Trim();
                collection = collection.Where(c => c.Name == name);
            }

            if(!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(a => a.Name.Contains(searchQuery)
                ||a.Description != null && a.Description.Contains(searchQuery)
                );
            }
            var totalItmeCount = await collection.CountAsync();
            var paginationMetadata = new PaginationMetadata(totalItmeCount,pageSize,pageNumber);
           var collectionToReturn = await collection.Skip(pageSize*(pageNumber-1)).Take(pageSize).OrderBy(c =>c.Name).ToListAsync();

            return (collectionToReturn,paginationMetadata);
        }
    }
}
