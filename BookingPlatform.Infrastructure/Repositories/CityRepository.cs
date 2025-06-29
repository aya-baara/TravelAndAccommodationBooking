using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingPlatform.Infrastructure.Repositories;

public class CityRepository : ICityRepository
{
    private readonly AppDbContext _context;

    public CityRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<City> CreateCityAsync(City city, CancellationToken cancellationToken = default)
    {
        var result = await _context.Cities.AddAsync(city, cancellationToken);
        return result.Entity;

    }

    public async Task DeleteCityAsync(Guid cityId, CancellationToken cancellationToken = default)
    {
        var city = await GetCityByIdAsync(cityId, cancellationToken);
        if (city != null)
        {
            _context.Cities.Remove(city);
        }
    }

    public async Task<City?> GetCityByIdAsync(Guid cityId, CancellationToken cancellationToken = default)
    {
        return await _context.Cities.FirstOrDefaultAsync(c => c.Id == cityId, cancellationToken);
    }

    public async Task<City?> GetCityByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Cities.FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
    }

    public async Task<List<City>> GetTopBookedCitiesAsync(int num, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
       .Where(b => b.Rooms.Any())
       .Select(b => b.Rooms.First().Hotel.City)
       .GroupBy(city => city.Id)
       .Select(g => new
       {
           City = g.First(),
           Count = g.Count()
       })
       .OrderByDescending(x => x.Count)
       .Take(num)
       .Select(x => x.City)
       .ToListAsync();
    }

    public async Task UpdateCityAsync(City city, CancellationToken cancellationToken = default)
    {
        _context.Cities.Update(city);
    }
}

