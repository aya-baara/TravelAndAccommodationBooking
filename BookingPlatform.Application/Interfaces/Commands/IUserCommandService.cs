using BookingPlatform.Application.Dtos.Users;

namespace BookingPlatform.Application.Interfaces.Commands;

public interface IUserCommandService
{
    Task SignUpAsync(CreateUserDto dto, CancellationToken ct);
}

