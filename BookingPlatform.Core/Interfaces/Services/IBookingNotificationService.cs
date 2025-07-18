using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Services;

public interface IBookingNotificationService
{
    Task SendBookingConfirmationAsync(Booking booking, CancellationToken ct);

}

