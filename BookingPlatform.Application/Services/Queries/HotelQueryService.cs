using AutoMapper;
using BookingPlatform.Application.Dtos.Hotels;
using BookingPlatform.Application.Dtos.Images;
using BookingPlatform.Application.Dtos.Reviews;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingPlatform.Application.Services.Queries;

public class HotelQueryService : IHotelQueryService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IImageRepository _imageRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<HotelQueryService> _logger;

    public HotelQueryService(IHotelRepository hotelRepository
        , IReviewRepository reviewRepository
        , IImageRepository imageRepository
        , IMapper mapper
        , ILogger<HotelQueryService> logger)
    {
        _hotelRepository = hotelRepository;
        _reviewRepository = reviewRepository;
        _imageRepository = imageRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<HotelDetailsDto> GetHotelDetailsByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting hotel details : {HotelId}", id);

        var hotel = await _hotelRepository.GetHotelByIdAsync(id, cancellationToken);
        if (hotel is null)
        {
            _logger.LogWarning($"Hotel with ID {id} not found");
            throw new NotFoundException("The requested hotel not found");
        }
        var reviews = await _reviewRepository.GetReviewsByHotelIdAsync(id);
        var images = await _imageRepository.GetHotelGalleryImagesAsync(id, cancellationToken);

        var dto = _mapper.Map<HotelDetailsDto>(hotel);
        dto.Images = _mapper.Map<List<ImageResponseDto>>(images);
        dto.Reviews = _mapper.Map<List<ReviewResponseDto>>(reviews.Items);

        _logger.LogInformation("Hotel details for ID {HotelId} retrieved successfully", id);

        return dto;
    }
}

