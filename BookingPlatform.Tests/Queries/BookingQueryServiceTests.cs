using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using BookingPlatform.Application.Dtos.Bookings;
using BookingPlatform.Application.Services.Queries;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookingPlatform.Tests.Queries;

public class BookingQueryServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly Mock<IImageRepository> _imageRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<BookingQueryService>> _loggerMock;
    private readonly BookingQueryService _service;

    public BookingQueryServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());

        _fixture.Behaviors
           .OfType<ThrowingRecursionBehavior>()
           .ToList()
           .ForEach(b => _fixture.Behaviors.Remove(b));

        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _fixture.Customize<DateOnly>(c =>
           c.FromFactory(() => DateOnly.FromDateTime(DateTime.Today.AddYears(-_fixture.Create<int>() % 40 - 20))));

        _bookingRepositoryMock = _fixture.Freeze<Mock<IBookingRepository>>();
        _imageRepositoryMock = _fixture.Freeze<Mock<IImageRepository>>();
        _userRepositoryMock = _fixture.Freeze<Mock<IUserRepository>>();
        _mapperMock = _fixture.Freeze<Mock<IMapper>>();
        _loggerMock = _fixture.Freeze<Mock<ILogger<BookingQueryService>>>();

        _service = new BookingQueryService(
            _bookingRepositoryMock.Object,
            _imageRepositoryMock.Object,
            _userRepositoryMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetBookingByIdAsync_ShouldReturnMappedBooking_WhenBookingExists()
    {
        // Arrange
        var booking = _fixture.Create<Booking>();
        var bookingDto = _fixture.Create<BookingResponseDto>();

        _bookingRepositoryMock
            .Setup(r => r.GetBookingById(booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        _mapperMock
            .Setup(m => m.Map<BookingResponseDto>(booking))
            .Returns(bookingDto);

        // Act
        var result = await _service.GetBookingByIdAsync(booking.Id, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(bookingDto);
    }

    [Fact]
    public async Task GetBookingByIdAsync_ShouldThrowNotFoundException_WhenBookingNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        _bookingRepositoryMock
            .Setup(r => r.GetBookingById(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        // Act
        Func<Task> act = async () => await _service.GetBookingByIdAsync(id, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("The Requested Booking Not found");
    }

    [Fact]
    public async Task GetRecentHotelsForUserAsync_ShouldReturnRecentHotels_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = _fixture.Create<User>();

        _userRepositoryMock
            .Setup(r => r.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var hotel = _fixture.Build<Hotel>()
            .With(h => h.City, _fixture.Create<City>())
            .Create();

        var booking = _fixture.Build<Booking>()
            .With(b => b.Rooms, new List<Room>
            {
                new Room { Hotel = hotel }
            })
            .With(b => b.CheckIn, DateTime.Today)
            .With(b => b.CheckOut, DateTime.Today.AddDays(3))
            .With(b => b.Invoice, new Invoice { TotalAmount = 200 })
            .Create();

        var bookings = new List<Booking> { booking };

        _bookingRepositoryMock
            .Setup(r => r.GetRecentlyBookingByUserIdAsync(userId, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        _imageRepositoryMock
            .Setup(r => r.GetHotelThumbnailImageAsync(hotel.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Image
            {
                 Path = "image_url", 
                 HotelId = hotel.Id 
             });

        // Act
        var result = await _service.GetRecentHotelsForUserAsync(userId, 1, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].HotelName.Should().Be(hotel.Name);
        result[0].CityName.Should().Be(hotel.City.Name);
        result[0].StarRating.Should().Be(hotel.StarRating);
        result[0].Thumbnail?.Path.Should().Be("image_url");
        result[0].CheckIn.Should().Be(booking.CheckIn);
        result[0].CheckOut.Should().Be(booking.CheckOut);
        result[0].TotalPrice.Should().Be(booking.Invoice.TotalAmount);
    }

    [Fact]
    public async Task GetRecentHotelsForUserAsync_ShouldThrowNotFoundException_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepositoryMock
            .Setup(r => r.GetUserByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        Func<Task> act = async () => await _service.GetRecentHotelsForUserAsync(userId, 1, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("The Requested User Not found");
    }
}
