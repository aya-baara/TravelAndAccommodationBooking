using AutoMapper;
using BookingPlatform.Application.Dtos.Hotels;
using BookingPlatform.Application.Dtos.Images;
using BookingPlatform.Application.Dtos.Reviews;
using BookingPlatform.Application.Services.Queries;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Enums;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Core.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Sieve.Services;

namespace BookingPlatform.Tests.Queries;

public class HotelQueryServiceTests
{
    private readonly Mock<IHotelRepository> _hotelRepoMock = new();
    private readonly Mock<IReviewRepository> _reviewRepoMock = new();
    private readonly Mock<IImageRepository> _imageRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<HotelQueryService>> _loggerMock = new();
    private readonly Mock<ISieveProcessor> _sieveMock = new();
    private readonly HotelQueryService _service;

    public HotelQueryServiceTests()
    {
        _service = new HotelQueryService(
            _hotelRepoMock.Object,
            _reviewRepoMock.Object,
            _imageRepoMock.Object,
            _mapperMock.Object,
            _loggerMock.Object,
            _sieveMock.Object);
    }

    [Fact]
    public async Task GetHotelDetailsByIdAsync_ShouldReturnHotelDetails_WhenHotelExists()
    {
        var hotelId = Guid.NewGuid();
        var hotel = new Hotel { Id = hotelId };
        var reviews = new PaginatedResult<Review> (  new List<Review> { new Review() },10,2,1 );
        var images = new List<Image> { new Image() };
        var hotelDto = new HotelDetailsDto();

        _hotelRepoMock.Setup(r => r.GetHotelByIdAsync(hotelId, It.IsAny<CancellationToken>())).ReturnsAsync(hotel);
        _reviewRepoMock
            .Setup(r => r.GetReviewsByHotelIdAsync(hotelId, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reviews);
        _imageRepoMock.Setup(r => r.GetHotelGalleryImagesAsync(hotelId, It.IsAny<CancellationToken>())).ReturnsAsync(images);
        _mapperMock.Setup(m => m.Map<HotelDetailsDto>(hotel)).Returns(hotelDto);
        _mapperMock.Setup(m => m.Map<List<ImageResponseDto>>(images)).Returns(new List<ImageResponseDto>());
        _mapperMock.Setup(m => m.Map<List<ReviewResponseDto>>(reviews.Items)).Returns(new List<ReviewResponseDto>());

        var result = await _service.GetHotelDetailsByIdAsync(hotelId, CancellationToken.None);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetHotelDetailsByIdAsync_ShouldThrowNotFoundException_WhenHotelDoesNotExist()
    {
        var hotelId = Guid.NewGuid();
        _hotelRepoMock.Setup(r => r.GetHotelByIdAsync(hotelId, It.IsAny<CancellationToken>())).ReturnsAsync((Hotel)null);

        var act = async () => await _service.GetHotelDetailsByIdAsync(hotelId, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("The requested hotel not found");
    }

    [Fact]
    public async Task GetFeaturedDealsAsync_ShouldReturnFeaturedDeals()
    {
        var deals = new List<FeaturedHotelProjection>
    {
        new()
        {
            HotelId = Guid.NewGuid(),
            HotelName = "Hotel 1",
            Location = "Loc",
            Thumbnail = new Image { Path = "img.jpg", Type = ImageType.Thumbnail },
            StarRating = 5,
            OriginalPrice = 200,
            DiscountedPrice = 150
        }
    };

        _hotelRepoMock
            .Setup(r => r.GetFeaturedDealsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deals);

        var result = await _service.GetFeaturedDealsAsync(1, CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].HotelName.Should().Be("Hotel 1");
        result[0].Thumbnail.Should().NotBeNull();
        result[0].Thumbnail.Type.Should().Be(ImageType.Thumbnail);
    }



}
