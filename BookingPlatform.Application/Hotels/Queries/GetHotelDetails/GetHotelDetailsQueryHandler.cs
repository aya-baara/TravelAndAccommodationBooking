using AutoMapper;
using BookingPlatform.Application.Hotels.Dtos;
using BookingPlatform.Application.Images.Dtos;
using BookingPlatform.Application.Reviews.Dtos;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using MediatR;

namespace BookingPlatform.Application.Hotels.Queries.GetHotelDetails;

public class GetHotelDetailsQueryHandler : IRequestHandler<GetHotelDetailsQuery, HotelDetailsDto>
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IImageRepository _imageRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public GetHotelDetailsQueryHandler(IHotelRepository hotelRepository
        , IImageRepository imageRepository
        , IReviewRepository reviewRepository
        , IMapper mapper)
    {
        _hotelRepository = hotelRepository;
        _imageRepository = imageRepository;
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<HotelDetailsDto> Handle(GetHotelDetailsQuery request, CancellationToken cancellationToken)
    {
        var hotel = await _hotelRepository.GetHotelByIdAsync(request.HotelId)
            ?? throw new NotFoundException("The requested hotel not found");
        var reviews = await _reviewRepository.GetReviewsByHotelIdAsync(request.HotelId);
        var images = await _imageRepository.GetHotelGalleryImagesAsync(request.HotelId);

        var dto = _mapper.Map<HotelDetailsDto>(hotel);
        dto.Images = _mapper.Map<List<ImageResponseDto>>(images);
        dto.Reviews = _mapper.Map<List<ReviewResponseDto>>(reviews.Items);

        return dto;

    }
}

