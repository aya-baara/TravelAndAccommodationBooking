using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Repositories;

public interface ICityRepository
{
    Task<City> CreateCityAsync(City city, CancellationToken cancellationToken = default);
    Task<City?> GetCityByIdAsync(Guid cityId, CancellationToken cancellationToken = default);
    Task<City?> GetCityByNameAsync(string name, CancellationToken cancellationToken = default);
    Task UpdateCityAsync(City city, CancellationToken cancellationToken = default);
    Task DeleteCityAsync(Guid cityId, CancellationToken cancellationToken = default);
    Task<List<City>> GetTopBookedCitiesAsync(int num, CancellationToken cancellationToken = default);

}

