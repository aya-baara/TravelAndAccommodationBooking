using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace BookingPlatform.Application.Services.Helpers;
public class BookingCreationService : IBookingCreationService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDiscountRepository _discountRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<BookingCreationService> _logger;

    public BookingCreationService(IRoomRepository roomRepository
        , IUserRepository userRepository
        , IDiscountRepository discountRepository
        , IBookingRepository bookingRepository
        , ILogger<BookingCreationService> logger)
    {
        _roomRepository = roomRepository;
        _userRepository = userRepository;
        _discountRepository = discountRepository;
        _bookingRepository = bookingRepository;
        _logger = logger;
    }

    public async Task<Booking> CreateBookingAsync(Booking _booking, CancellationToken cancellationToken)
    {
        decimal totalBefore = 0;
        decimal totalAfter = 0;
        var now = DateTime.UtcNow;
        var rooms = new List<Room>();

        var user = await _userRepository.GetUserByIdAsync(_booking.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning($"Attempted to Add Booking to non-existent User with ID {_booking.UserId}");
            throw new NotFoundException("The Requested User Not found");
        }

        foreach (var room in _booking.Rooms)
        {
            var fullRoom = await _roomRepository.GetRoomByIdAsync(room.Id, cancellationToken);
            if (fullRoom is null)
            {
                _logger.LogWarning($"Attempted to Add Booking to non-existent Room with ID {room.Id}");
                throw new NotFoundException("The Requested Room Not found");
            }
            var available = await _bookingRepository.IsRoomAvailableAsync(room.Id, _booking.CheckIn, _booking.CheckOut, cancellationToken);
            if (!available)
            {
                _logger.LogWarning($"Room with ID {room.Id} is not available from {_booking.CheckIn:yyyy-MM-dd} to {_booking.CheckOut:yyyy-MM-dd}");
                throw new ConflictException($"Room {room.Id} is not available during selected dates.");
            }
            rooms.Add(fullRoom);

            var nights = (_booking.CheckOut - _booking.CheckIn).Days;
            var basePrice = fullRoom.PricePerNight * nights;
            totalBefore += basePrice;

            var discount = await _discountRepository.GetValidDiscountForRoomAsync(room.Id, _booking.CheckIn, _booking.CheckOut, cancellationToken);
            if (discount != null)
            {
                var discounted = basePrice * (1 - discount.Percentage / 100m);
                totalAfter += discounted;
            }
            else
            {
                totalAfter += basePrice;
            }
        }

        var booking = new Booking
        {
            UserId = _booking.UserId,
            Rooms = rooms,
            Remarks = _booking.Remarks,
            CheckIn = _booking.CheckIn,
            CheckOut = _booking.CheckOut,
            BookingDate = _booking.BookingDate,
            TotalPriceBeforeDiscount = totalBefore,
            TotalPriceAfterDiscount = totalAfter
        };

        await _bookingRepository.CreateBookingAsync(booking, cancellationToken);

        _logger.LogInformation($"Booking created successfully for user {booking.UserId} from {booking.CheckIn:yyyy-MM-dd} to {booking.CheckOut:yyyy-MM-dd}, total: {totalAfter:C}");

        return booking;
    }
}
