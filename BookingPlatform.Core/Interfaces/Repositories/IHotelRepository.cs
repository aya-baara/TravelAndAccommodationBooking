using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Repositories;

public interface IHotelRepository
{
    Task CreateHotelAsync(Hotel hotel);
    Task<Hotel?> GetHotelByIdAsync(Guid hotelId);
    Task<PaginatedResult<Hotel>> GetHotelsByCityIdAsync(Guid cityId, int page, int size);
    Task UpdateHotelAsync(Hotel hotel);
    Task DeleteHotelByIdAsync(Guid hotelId);
    Task UpdateHotelRateAsync(Guid hotelId, double newRate);
}

