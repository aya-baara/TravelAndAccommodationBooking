using AutoFixture;
using AutoMapper;
using BookingPlatform.Application.Dtos.Bookings;
using BookingPlatform.Application.Services.Commands;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookingPlatform.Tests.Commands;

public class BookingCommandServiceTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IBookingCreationService> _bookingCreationServiceMock = new();
    private readonly Mock<IBookingNotificationService> _notificationServiceMock = new();
    private readonly Mock<IBookingRepository> _bookingRepositoryMock = new();
    private readonly Mock<IRoomRepository> _roomRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IMapper _mapper;
    private readonly ILogger<BookingCommandService> _logger;
    private readonly BookingCommandService _service;

    public BookingCommandServiceTests()
    {
        _fixture.Behaviors
        .OfType<ThrowingRecursionBehavior>()
        .ToList()
        .ForEach(b => _fixture.Behaviors.Remove(b));

        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _fixture.Customize<DateOnly>(c =>
            c.FromFactory(() => DateOnly.FromDateTime(DateTime.Today.AddYears(-_fixture.Create<int>() % 40 - 20))));

        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Booking, BookingResponseDto>();
            cfg.CreateMap<UpdateBookingDto, Booking>();
        }));

        _logger = new LoggerFactory().CreateLogger<BookingCommandService>();

        _service = new BookingCommandService(
            _bookingCreationServiceMock.Object,
            _notificationServiceMock.Object,
            _bookingRepositoryMock.Object,
            _roomRepositoryMock.Object,
            _mapper,
            _logger,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldCreateBookingSuccessfully()
    {
        var userId = Guid.NewGuid();
        var roomId = Guid.NewGuid();

        var createDto = _fixture.Build<CreateBookingDto>()
                                .With(x => x.RoomIds, new List<Guid> { roomId })
                                .With(x => x.Remarks, "Test remark")
                                .With(x => x.CheckIn, DateTime.Today.AddDays(5))
                                .With(x => x.CheckOut, DateTime.Today.AddDays(10))
                                .Create();

        var room = _fixture.Build<Room>().With(r => r.Id, roomId).Create();
        var booking = _fixture.Build<Booking>()
                              .With(b => b.Id, Guid.NewGuid())
                              .With(b => b.UserId, userId)
                              .With(b => b.Rooms, new List<Room> { room })
                              .With(b => b.Remarks, createDto.Remarks)
                              .With(b => b.CheckIn, createDto.CheckIn)
                              .With(b => b.CheckOut, createDto.CheckOut)
                              .With(b => b.BookingDate, DateOnly.FromDateTime(DateTime.UtcNow))
                              .Create();

        _roomRepositoryMock.Setup(r => r.GetRoomByIdAsync(roomId, It.IsAny<CancellationToken>())).ReturnsAsync(room);

        _bookingCreationServiceMock.Setup(b => b.CreateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).ReturnsAsync(booking);

        _notificationServiceMock.Setup(n => n.SendBookingConfirmationAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _service.CreateBookingAsync(createDto, userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(booking.Id, result.Id);

        _roomRepositoryMock.Verify(r => r.GetRoomByIdAsync(roomId, It.IsAny<CancellationToken>()), Times.Once);
        _bookingCreationServiceMock.Verify(b => b.CreateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
        _notificationServiceMock.Verify(n => n.SendBookingConfirmationAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldThrowNotFound_WhenRoomDoesNotExist()
    {
        var userId = Guid.NewGuid();
        var roomId = Guid.NewGuid();

        var createDto = _fixture.Build<CreateBookingDto>()
                                .With(x => x.RoomIds, new List<Guid> { roomId })
                                .Create();

        _roomRepositoryMock.Setup(r => r.GetRoomByIdAsync(roomId, It.IsAny<CancellationToken>())).ReturnsAsync((Room?)null);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateBookingAsync(createDto, userId, CancellationToken.None));

        Assert.Contains(roomId.ToString(), ex.Message);
    }

    [Fact]
    public async Task DeleteBookingAsync_ShouldDelete_WhenBookingExistsAndUserAuthorized_AndWithinOneDay_AndCheckInInFuture()
    {
        var bookingId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var bookingDate = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(-23));
        var checkInDate = DateTime.UtcNow.AddDays(2);

        var booking = _fixture.Build<Booking>()
                              .With(b => b.Id, bookingId)
                              .With(b => b.UserId, userId)
                              .With(b => b.BookingDate, bookingDate)
                              .With(b => b.CheckIn, checkInDate)
                              .With(b => b.CheckOut, checkInDate.AddDays(3))
                              .Create();

        _bookingRepositoryMock.Setup(b => b.GetBookingById(bookingId, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
        _bookingRepositoryMock.Setup(b => b.DeleteBookingAsync(bookingId, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _service.DeleteBookingAsync(bookingId, userId, CancellationToken.None);

        _bookingRepositoryMock.Verify(b => b.DeleteBookingAsync(bookingId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteBookingAsync_ShouldThrowNotFound_WhenBookingNotExists()
    {
        var bookingId = Guid.NewGuid();

        _bookingRepositoryMock.Setup(b => b.GetBookingById(bookingId, It.IsAny<CancellationToken>())).ReturnsAsync((Booking?)null);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteBookingAsync(bookingId, Guid.NewGuid(), CancellationToken.None));

        Assert.Equal("The Requested Booking Not found", ex.Message);
    }

    [Fact]
    public async Task DeleteBookingAsync_ShouldThrowForbidden_WhenUserNotOwner()
    {
        var bookingId = Guid.NewGuid();
        var ownerUserId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var booking = _fixture.Build<Booking>()
                              .With(b => b.Id, bookingId)
                              .With(b => b.UserId, ownerUserId)
                              .With(b => b.BookingDate, DateOnly.FromDateTime(DateTime.UtcNow))
                              .With(b => b.CheckIn, DateTime.UtcNow.AddDays(5))
                              .Create();

        _bookingRepositoryMock.Setup(b => b.GetBookingById(bookingId, It.IsAny<CancellationToken>())).ReturnsAsync(booking);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _service.DeleteBookingAsync(bookingId, otherUserId, CancellationToken.None));

        Assert.Equal("You are not allowed to access this booking.", ex.Message);
    }

    [Fact]
    public async Task DeleteBookingAsync_ShouldThrowConflict_WhenDeletionWindowExpired()
    {
        var bookingId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var oldBookingDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-2));
        var booking = _fixture.Build<Booking>()
                              .With(b => b.Id, bookingId)
                              .With(b => b.UserId, userId)
                              .With(b => b.BookingDate, oldBookingDate)
                              .With(b => b.CheckIn, DateTime.UtcNow.AddDays(5))
                              .Create();

        _bookingRepositoryMock.Setup(b => b.GetBookingById(bookingId, It.IsAny<CancellationToken>())).ReturnsAsync(booking);

        var ex = await Assert.ThrowsAsync<ConflictException>(() => _service.DeleteBookingAsync(bookingId, userId, CancellationToken.None));

        Assert.Equal("You can delete a booking only within one day after booking creation.", ex.Message);
    }

    [Fact]
    public async Task DeleteBookingAsync_ShouldThrowConflict_WhenCheckInDatePassed()
    {
        var bookingId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var bookingDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var pastCheckIn = DateTime.UtcNow.AddDays(-1);

        var booking = _fixture.Build<Booking>()
                              .With(b => b.Id, bookingId)
                              .With(b => b.UserId, userId)
                              .With(b => b.BookingDate, bookingDate)
                              .With(b => b.CheckIn, pastCheckIn)
                              .Create();

        _bookingRepositoryMock.Setup(b => b.GetBookingById(bookingId, It.IsAny<CancellationToken>())).ReturnsAsync(booking);

        var ex = await Assert.ThrowsAsync<ConflictException>(() => _service.DeleteBookingAsync(bookingId, userId, CancellationToken.None));

        Assert.Equal("Cannot delete bookings where check-in date has started or passed.", ex.Message);
    }

    [Fact]
    public async Task UpdateBookingAsync_ShouldUpdate_WhenBookingExistsAndUserAuthorized()
    {
        var dto = _fixture.Create<UpdateBookingDto>();
        var userId = Guid.NewGuid();

        var booking = _fixture.Build<Booking>()
                              .With(b => b.Id, dto.Id)
                              .With(b => b.UserId, userId)
                              .Create();

        _bookingRepositoryMock.Setup(b => b.GetBookingById(dto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
        _bookingRepositoryMock.Setup(b => b.UpdateBookingAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _service.UpdateBookingAsync(dto, userId, CancellationToken.None);

        _bookingRepositoryMock.Verify(b => b.UpdateBookingAsync(booking, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateBookingAsync_ShouldThrowNotFound_WhenBookingDoesNotExist()
    {
        var dto = _fixture.Create<UpdateBookingDto>();
        var userId = Guid.NewGuid();

        _bookingRepositoryMock.Setup(b => b.GetBookingById(dto.Id, It.IsAny<CancellationToken>())).ReturnsAsync((Booking?)null);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateBookingAsync(dto, userId, CancellationToken.None));

        Assert.Equal("The Requested Booking Not found", ex.Message);
    }

    [Fact]
    public async Task UpdateBookingAsync_ShouldThrowForbidden_WhenUserNotOwner()
    {
        var dto = _fixture.Create<UpdateBookingDto>();
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var booking = _fixture.Build<Booking>()
                              .With(b => b.Id, dto.Id)
                              .With(b => b.UserId, ownerId)
                              .Create();

        _bookingRepositoryMock.Setup(b => b.GetBookingById(dto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(booking);

        var ex = await Assert.ThrowsAsync<ForbiddenAccessException>(() => _service.UpdateBookingAsync(dto, otherUserId, CancellationToken.None));

        Assert.Equal("You are not allowed to access this Booking.", ex.Message);
    }
}
