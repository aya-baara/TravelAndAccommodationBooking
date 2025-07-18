using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Services;

public interface IBookingCreationService
{
    Task<Booking> CreateBookingAsync(Booking booking, CancellationToken ct);
}

