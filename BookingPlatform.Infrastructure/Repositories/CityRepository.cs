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

    public async Task CreateCityAsync(City city)
    {
        await _context.Cities.AddAsync(city);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCityAsync(Guid cityId)
    {
        var city = await GetCityByIdAsync(cityId);
        if (city != null)
        {
            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<City?> GetCityByIdAsync(Guid cityId)
    {
        return await _context.Cities.FirstOrDefaultAsync(c => c.Id == cityId);
    }

    public async Task<City?> GetCityByNameAsync(string name)
    {
        return await _context.Cities.FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<List<City>> GetTopBookedCitiesAsync(int num)
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

    public async Task UpdateCityAsync(City city)
    {
        _context.Cities.Update(city);
        await _context.SaveChangesAsync();
    }
}

