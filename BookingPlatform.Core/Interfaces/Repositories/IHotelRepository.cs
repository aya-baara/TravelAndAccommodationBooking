using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Models;

namespace BookingPlatform.Core.Interfaces.Repositories;

public interface IHotelRepository
{
    Task<Hotel> CreateHotelAsync(Hotel hotel, CancellationToken cancellationToken = default);
    Task<Hotel?> GetHotelByIdAsync(Guid hotelId, CancellationToken cancellationToken = default);
    Task<PaginatedResult<Hotel>> GetHotelsByCityIdAsync(Guid cityId, int page = 1, int size = 20
        , CancellationToken cancellationToken = default);
    Task UpdateHotelAsync(Hotel hotel, CancellationToken cancellationToken = default);
    Task DeleteHotelByIdAsync(Guid hotelId, CancellationToken cancellationToken = default);
    Task UpdateHotelRateAsync(Guid hotelId, double newRate, CancellationToken cancellationToken = default);
    Task<HotelRatingStats?> GetRatingStatsByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken = default);
}

