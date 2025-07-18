using AutoFixture;
using AutoMapper;
using BookingPlatform.Application.Dtos.Rooms;
using BookingPlatform.Application.Services.Queries;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Sieve.Services;

namespace BookingPlatform.Tests.Queries;


public class RoomQueryServiceTests
{
    private readonly Mock<IRoomRepository> _roomRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<RoomQueryService>> _loggerMock = new();
    private readonly Mock<ISieveProcessor> _sieveMock = new();
    private readonly RoomQueryService _service;
    private readonly Fixture _fixture;

    public RoomQueryServiceTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors
        .OfType<ThrowingRecursionBehavior>()
        .ToList()
        .ForEach(b => _fixture.Behaviors.Remove(b));

        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _fixture.Customize<DateOnly>(c =>
           c.FromFactory(() => DateOnly.FromDateTime(DateTime.Today.AddYears(-_fixture.Create<int>() % 40 - 20))));

        _service = new RoomQueryService(
            _roomRepositoryMock.Object,
            _mapperMock.Object,
            _loggerMock.Object,
            _sieveMock.Object
        );
    }

    [Fact]
    public async Task GetRoomByIdAsync_ShouldReturnMappedDto_WhenRoomExists()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var room = _fixture.Build<Room>().With(r => r.Id, roomId).Create();
        var dto = _fixture.Create<RoomResponseDto>();

        _roomRepositoryMock.Setup(r => r.GetRoomByIdAsync(roomId, default)).ReturnsAsync(room);
        _mapperMock.Setup(m => m.Map<RoomResponseDto>(room)).Returns(dto);

        // Act
        var result = await _service.GetRoomByIdAsync(roomId, default);

        // Assert
        Assert.Equal(dto, result);
    }

    [Fact]
    public async Task GetRoomByIdAsync_ShouldThrowNotFoundException_WhenRoomIsNull()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        _roomRepositoryMock.Setup(r => r.GetRoomByIdAsync(roomId, default)).ReturnsAsync((Room)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.GetRoomByIdAsync(roomId, default));
    }

    [Fact]
    public async Task GetAvailableRoomsByHotelAsync_ShouldReturnMappedDtos()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var rooms = _fixture.CreateMany<Room>(3).ToList();
        var roomDtos = _fixture.CreateMany<RoomResponseDto>(3).ToList();

        _roomRepositoryMock.Setup(r => r.GetAvailbleRoomsByHotelId(hotelId, default)).ReturnsAsync(rooms);
        _mapperMock.Setup(m => m.Map<List<RoomResponseDto>>(rooms)).Returns(roomDtos);

        // Act
        var result = await _service.GetAvailableRoomsByHotelAsync(hotelId, default);

        // Assert
        Assert.Equal(roomDtos, result);
    }
}
