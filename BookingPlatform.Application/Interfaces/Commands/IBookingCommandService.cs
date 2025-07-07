using BookingPlatform.Application.Dtos.Bookings;

namespace BookingPlatform.Application.Interfaces.Commands;

public interface IBookingCommandService
{
    Task<BookingResponseDto> createBookingAsync(CreateBookingDto dto, CancellationToken cancellationToken);
    Task DeleteBookingAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateBookingAsync(UpdateBookingDto dto, CancellationToken cancellationToken);
}

