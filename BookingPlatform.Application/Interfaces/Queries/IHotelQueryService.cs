using BookingPlatform.Application.Dtos.Hotels;

namespace BookingPlatform.Application.Interfaces.Queries;

public interface IHotelQueryService
{
    Task<HotelDetailsDto> GetHotelDetailsByIdAsync(Guid id, CancellationToken cancellationToken);
}

