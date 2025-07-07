using BookingPlatform.Application.Dtos.Cities;

namespace BookingPlatform.Application.Interfaces.Commands;

public interface ICityCommandService
{
    Task<CityResponseDto> CreateCityAsync(CreateCityDto dto, CancellationToken cancellationToken);
    Task UpdateCityAsync(UpdateCityDto dto, CancellationToken cancellationToken);
    Task DeleteCityAsync(Guid id, CancellationToken cancellationToken);
}

