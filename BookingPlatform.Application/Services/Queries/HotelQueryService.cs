using AutoMapper;
using BookingPlatform.Application.Dtos.Hotels;
using BookingPlatform.Application.Dtos.Images;
using BookingPlatform.Application.Dtos.Reviews;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Sieve.Services;
using Microsoft.EntityFrameworkCore;
using BookingPlatform.Application.SieveConfigurations;

namespace BookingPlatform.Application.Services.Queries;

public class HotelQueryService : IHotelQueryService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IImageRepository _imageRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<HotelQueryService> _logger;
    private readonly ISieveProcessor _sieve;

    public HotelQueryService(IHotelRepository hotelRepository
        , IReviewRepository reviewRepository
        , IImageRepository imageRepository
        , IMapper mapper
        , ILogger<HotelQueryService> logger
        , ISieveProcessor sieveProcessor)
    {
        _hotelRepository = hotelRepository;
        _reviewRepository = reviewRepository;
        _imageRepository = imageRepository;
        _mapper = mapper;
        _logger = logger;
        _sieve = sieveProcessor;
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

        _logger.LogInformation("Retrieved {ReviewCount} reviews and {ImageCount} images for hotel ID {HotelId}",
        reviews?.Items?.Count ?? 0, images?.Count ?? 0, id);

        return dto;
    }
    public async Task<List<FeaturedHotelDto>> GetFeaturedDealsAsync(int count, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching {Count} featured hotel deals", count);

        var projections = await _hotelRepository.GetFeaturedDealsAsync(count, cancellationToken);

        _logger.LogInformation("Retrieved {Count} featured hotel deals successfully", projections.Count);

        return projections.Select(p => new FeaturedHotelDto
        {
            HotelId = p.HotelId,
            HotelName = p.HotelName,
            Location = p.Location,
            Thumbnail = p.Thumbnail,
            StarRating = p.StarRating,
            OriginalPrice = p.OriginalPrice,
            DiscountedPrice = p.DiscountedPrice
        }).ToList();
    }
    public async Task<PaginatedResult<HotelSearchDto>> SearchHotelsAsync(HotelSearchRequest request, CancellationToken ct)
    {
        _logger.LogInformation("Searching hotels with filters: CheckIn={CheckIn}, CheckOut={CheckOut}, Adults={Adults}, Children={Children}, Rooms={Rooms}",
            request.CheckIn, request.CheckOut, request.Adults, request.Children, request.Rooms);

        var query = _hotelRepository.GetAllAsQueryable();

        if (request.CheckIn.HasValue && request.CheckOut.HasValue)
        {
            var checkIn = request.CheckIn.Value;
            var checkOut = request.CheckOut.Value;
            var adults = request.Adults;
            var children = request.Children;

            query = query.Where(h =>
                h.Rooms.Any(r =>
                    r.AdultCapacity >= adults &&
                    r.ChildrenCapacity >= children &&
                    r.Bookings.All(b =>
                        b.CheckOut <= checkIn || b.CheckIn >= checkOut
                    )
                )
            );
        }

        if (request.Rooms > 1)
        {
            query = query.Where(h =>
                h.Rooms.Count(r =>
                    r.Bookings.All(b =>
                        b.CheckOut <= request.CheckIn || b.CheckIn >= request.CheckOut
                    )
                ) >= request.Rooms
            );
        }

        var filtered = _sieve.Apply(request, query);

        var total = await filtered.CountAsync(ct);
        var data = await filtered.ToListAsync(ct);

        _logger.LogInformation("Hotel search completed. Total results: {Total}", total);

        return new PaginatedResult<HotelSearchDto>(
            _mapper.Map<List<HotelSearchDto>>(data),
            total,
            request.Page ?? 1,
            request.PageSize ?? 10
        );
    }

    public async Task<PaginatedResult<HotelManagementDto>> SearchHotelsAdminAsync(HotelAdminSearchRequest request, CancellationToken ct)
    {
        _logger.LogInformation("Admin hotel search initiated. Filters: Page={Page}, PageSize={PageSize}",
            request.Page, request.PageSize);

        var query = _hotelRepository.GetAllAsQueryable(); // IQueryable<Hotel>

        var filtered = _sieve.Apply(request, query, applySorting: true, applyFiltering: true);

        var total = await filtered.CountAsync(ct);

        var paged = filtered
            .Skip(((request.Page ?? 1) - 1) * (request.PageSize ?? 10))
            .Take(request.PageSize ?? 10);

        _logger.LogInformation("Final SQL: {Query}", filtered.ToQueryString());

        var data = await paged.Select(h => new HotelManagementDto
        {
            Name = h.Name,
            StarRating = h.StarRating,
            OwnerName = h.Owner.FirstName,
            RoomCount = h.Rooms.Count,
            CreatedAt = h.CreatedAt,
            ModifiedAt = h.ModifiedAt
        }).ToListAsync(ct);


        _logger.LogInformation("Admin hotel search completed. Total matches: {Total}", total);

        return new PaginatedResult<HotelManagementDto>(data, total, request.Page ?? 1, request.PageSize ?? 10);
    }


}


