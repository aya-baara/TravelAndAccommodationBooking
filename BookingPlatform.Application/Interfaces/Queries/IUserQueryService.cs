using BookingPlatform.Application.Dtos.Users;

namespace BookingPlatform.Application.Interfaces.Queries;

public interface IUserQueryService
{
    Task<TokenResponseDto> LogInAsync(LogInDto dto, CancellationToken ct);

}

