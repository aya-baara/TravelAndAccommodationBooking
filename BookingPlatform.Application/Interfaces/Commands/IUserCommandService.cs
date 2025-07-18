using BookingPlatform.Application.Dtos.Users;

namespace BookingPlatform.Application.Interfaces.Commands;

public interface IUserCommandService
{
    Task SignUpAsync(SignUpDto dto, CancellationToken ct);
}

