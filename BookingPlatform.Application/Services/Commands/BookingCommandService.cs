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
    private readonly IMapper _mapper;
    private readonly ILogger<BookingCommandService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public BookingCommandService(
        IBookingCreationService bookingFactory
        , IBookingNotificationService bookingNotificationService
        , IBookingRepository bookingRepository
        , IMapper mapper
        , ILogger<BookingCommandService> logger
        , IUnitOfWork unitOfWork)
    {
        _bookingCreationService = bookingFactory;
        _notificationService = bookingNotificationService;
        _bookingRepository = bookingRepository;
        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<BookingResponseDto> createBookingAsync(CreateBookingDto dto, CancellationToken cancellationToken)
    {
        var bookingToCreate = _mapper.Map<Booking>(dto);
        var booking = await _bookingCreationService.CreateBookingAsync(bookingToCreate, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
        await _notificationService.SendBookingConfirmationAsync(booking, cancellationToken);
        return _mapper.Map<BookingResponseDto>(booking);
    }

    public async Task DeleteBookingAsync(Guid id, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetBookingById(id, cancellationToken);
        if (booking is null)
        {
            _logger.LogWarning($"Attempted to Delete non-existent Booking {id}");
            throw new NotFoundException("The Requested Booking Not found");
        }
        await _bookingRepository.DeleteBookingAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Booking Deleted successfully with ID {id}");
    }

    public async Task UpdateBookingAsync(UpdateBookingDto dto, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetBookingById(dto.Id, cancellationToken);
        if (booking is null)
        {
            _logger.LogWarning($"Attempted to Delete non-existent Booking {dto.Id}");
            throw new NotFoundException("The Requested Booking Not found");
        }

        _mapper.Map(dto, booking);
        await _bookingRepository.UpdateBookingAsync(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Booking updated successfully with ID {dto.Id}");
    }
}

