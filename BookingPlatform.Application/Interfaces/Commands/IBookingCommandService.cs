using BookingPlatform.Application.Dtos.Bookings;

namespace BookingPlatform.Application.Interfaces.Commands;

public interface IBookingCommandService
{
    Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto request, Guid userId, CancellationToken cancellationToken);
    Task DeleteBookingAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    Task UpdateBookingAsync(UpdateBookingDto dto, Guid userId, CancellationToken cancellationToken);
}

