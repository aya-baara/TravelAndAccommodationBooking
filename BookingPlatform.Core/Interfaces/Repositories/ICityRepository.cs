using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Repositories;

public interface ICityRepository
{
    Task CreateCityAsync(City city);
    Task<City> GetCityByIdAsync(Guid cityId);
    Task<City> GetCityByNameAsync(string name);
    Task UpdateCityAsync(City city);
    Task DeleteCityAsync(Guid cityId);
    Task<List<City>> GetTopBookedCitiesAsync(int num);

}

