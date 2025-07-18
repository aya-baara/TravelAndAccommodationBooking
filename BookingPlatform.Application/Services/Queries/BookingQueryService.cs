using AutoMapper;
using BookingPlatform.Application.Dtos.Bookings;
using BookingPlatform.Application.Dtos.Hotels;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace BookingPlatform.Application.Services.Queries;

public class BookingQueryService : IBookingQueryService
{
    private IBookingRepository _bookingRepository;
    private readonly IImageRepository _imageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<BookingQueryService> _logger;

    public BookingQueryService(IBookingRepository bookingRepository
        , IImageRepository imageRepository
        , IUserRepository userRepository
        , IMapper mapper
        , ILogger<BookingQueryService> logger)
    {
        _bookingRepository = bookingRepository;
        _imageRepository = imageRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BookingResponseDto> GetBookingByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetBookingById(id, cancellationToken);
        if (booking is null)
        {
            _logger.LogWarning($"Booking with ID {id} not found");
            throw new NotFoundException("The Requested Booking Not found");
        }

        _logger.LogInformation($"Successfully retrieved Booking with ID {id}");

        return _mapper.Map<BookingResponseDto>(booking);
    }

    public async Task<List<RecentHotelDto>> GetRecentHotelsForUserAsync(Guid userId, int count, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning($"User with ID {userId} not found");
            throw new NotFoundException("The Requested User Not found");
        }

        var bookings = await _bookingRepository.GetRecentlyBookingByUserIdAsync(userId, count, cancellationToken);

        var hotels = new List<RecentHotelDto>();

        foreach (var booking in bookings)
        {
            var firstRoom = booking.Rooms.First();
            var hotel = firstRoom.Hotel;

            hotels.Add(new RecentHotelDto
            {
                HotelName = hotel.Name,
                CityName = hotel.City?.Name ?? "",
                StarRating = hotel.StarRating,
                Thumbnail = await _imageRepository.GetHotelThumbnailImageAsync(hotel.Id, cancellationToken),
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                TotalPrice = booking.Invoice?.TotalAmount ?? booking.TotalPriceAfterDiscount
            });
        }
        _logger.LogInformation($"Successfully retrieved Recenlt booked hoteld for user with ID {userId}");

        return hotels;
    }

}

