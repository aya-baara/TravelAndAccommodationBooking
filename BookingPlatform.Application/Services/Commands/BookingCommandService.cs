using AutoMapper;
using BookingPlatform.Application.Dtos.Bookings;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace BookingPlatform.Application.Services.Commands;

public class BookingCommandService : IBookingCommandService
{
    private readonly IBookingCreationService _bookingCreationService;
    private readonly IBookingNotificationService _notificationService;
    private readonly IBookingRepository _bookingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<BookingCommandService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public BookingCommandService(
        IBookingCreationService bookingFactory
        , IBookingNotificationService bookingNotificationService
        , IBookingRepository bookingRepository
        ,IRoomRepository roomRepository
        , IMapper mapper
        , ILogger<BookingCommandService> logger
        , IUnitOfWork unitOfWork)
    {
        _bookingCreationService = bookingFactory;
        _notificationService = bookingNotificationService;
        _bookingRepository = bookingRepository;
        _roomRepository = roomRepository;
        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto request, Guid userId, CancellationToken cancellationToken)
    {
        var rooms = await GetRoomsByIdsAsync(request.RoomIds, cancellationToken);

        var bookingToCreate = new Booking
        {
            UserId = userId,
            Rooms = rooms,
            Remarks = request.Remarks ?? string.Empty,
            CheckIn = request.CheckIn,
            CheckOut = request.CheckOut,
            BookingDate = DateOnly.FromDateTime(DateTime.UtcNow),
        };
        var booking = await _bookingCreationService.CreateBookingAsync(bookingToCreate, cancellationToken);
        await _notificationService.SendBookingConfirmationAsync(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<BookingResponseDto>(booking);
    }

    private async Task<List<Room>> GetRoomsByIdsAsync(IEnumerable<Guid> roomIds, CancellationToken ct)
    {
        var rooms = new List<Room>();

        foreach (var roomId in roomIds)
        {
            var room = await _roomRepository.GetRoomByIdAsync(roomId, ct);
            if (room is null)
                throw new NotFoundException($"Room with ID {roomId} not found");
            rooms.Add(room);
        }
        return rooms;
    }

    public async Task DeleteBookingAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetBookingById(id, cancellationToken);
        if (booking is null)
        {
            _logger.LogWarning($"Attempted to Delete non-existent Booking {id}");
            throw new NotFoundException("The Requested Booking Not found");
        }
        if (booking.UserId != userId)
        {
            _logger.LogWarning($"Unauthorized attempt to Delete Booking {id} by user {userId}");
            throw new ForbiddenAccessException("You are not allowed to access this booking.");
        }

        var now = DateTime.UtcNow.Date;
        var bookingDate = booking.BookingDate.ToDateTime(TimeOnly.MinValue);

        // Allow deletion only if it's been **at most one day** since the booking date
        if ((now - bookingDate).TotalDays > 1)
        {
            _logger.LogWarning($"Attempt to delete booking {id} failed because deletion window expired.");
            throw new ConflictException("You can delete a booking only within one day after booking creation.");
        }

        // Check that CheckIn date is in the future (not today or past)
        if (booking.CheckIn.Date <= now)
        {
            _logger.LogWarning($"Attempt to delete booking {id} failed because CheckIn date is already today or past.");
            throw new ConflictException("Cannot delete bookings where check-in date has started or passed.");
        }

        await _bookingRepository.DeleteBookingAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Booking Deleted successfully with ID {id}");
    }


    public async Task UpdateBookingAsync(UpdateBookingDto dto, Guid userId,CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetBookingById(dto.Id, cancellationToken);
        if (booking is null)
        {
            _logger.LogWarning($"Attempted to Delete non-existent Booking {dto.Id}");
            throw new NotFoundException("The Requested Booking Not found");
        }
        if (booking.UserId != userId)
        {
            _logger.LogWarning($"Unauthorized attempt to Update Booking  {dto.Id} by user {userId}");
            throw new ForbiddenAccessException("You are not allowed to access this Booking.");
        }

        _mapper.Map(dto, booking);
        await _bookingRepository.UpdateBookingAsync(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Booking updated successfully with ID {dto.Id}");
    }
}

