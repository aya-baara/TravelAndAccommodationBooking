using BookingPlatform.Application.Dtos.Hotels;

namespace BookingPlatform.Application.Interfaces.Commands;

public interface IHotelCommandService
{
    Task<HotelResponseDto> CreateHotelAsync(CreateHotelDto dto,CancellationToken cancellationToken);
    Task UpdateHotelAsync(UpdateHotelDto dto, CancellationToken cancellationToken);
    Task DeleteHotelAsync(Guid id, CancellationToken cancellationToken);
}

