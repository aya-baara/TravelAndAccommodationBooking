using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using BookingPlatform.Application.Dtos.Rooms;
using BookingPlatform.Application.Services.Commands;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces;
using BookingPlatform.Core.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookingPlatform.Tests.Commands;

public class RoomCommandServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRoomRepository> _roomRepoMock;
    private readonly Mock<IHotelRepository> _hotelRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<RoomCommandService>> _loggerMock;
    private readonly RoomCommandService _sut;

    public RoomCommandServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Customize<DateOnly>(c =>
           c.FromFactory(() => DateOnly.FromDateTime(DateTime.Today.AddYears(-_fixture.Create<int>() % 40 - 20)))
       );
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _roomRepoMock = new Mock<IRoomRepository>();
        _hotelRepoMock = new Mock<IHotelRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<RoomCommandService>>();

        _sut = new RoomCommandService(
            _roomRepoMock.Object,
            _hotelRepoMock.Object,
            _mapperMock.Object,
            _loggerMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task CreateRoomAsync_ShouldReturnRoomResponseDto_WhenHotelExists()
    {
        // Arrange
        var dto = _fixture.Create<CreateRoomDto>();
        var hotel = _fixture.Create<Hotel>();
        var roomEntity = _fixture.Create<Room>();
        var roomResponse = _fixture.Create<RoomResponseDto>();

        _hotelRepoMock.Setup(h => h.GetHotelByIdAsync(dto.HotelId, default))
            .ReturnsAsync(hotel);

        _mapperMock.Setup(m => m.Map<Room>(dto))
            .Returns(roomEntity);

        _roomRepoMock.Setup(r => r.CreateRoomAsync(roomEntity, default))
            .ReturnsAsync(roomEntity);

        _mapperMock.Setup(m => m.Map<RoomResponseDto>(roomEntity))
            .Returns(roomResponse);

        // Act
        var result = await _sut.CreateRoomAsync(dto, default);

        // Assert
        result.Should().BeEquivalentTo(roomResponse);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateRoomAsync_ShouldThrowNotFoundException_WhenHotelDoesNotExist()
    {
        // Arrange
        var dto = _fixture.Create<CreateRoomDto>();

        _hotelRepoMock.Setup(h => h.GetHotelByIdAsync(dto.HotelId, default))
            .ReturnsAsync((Hotel)null);

        // Act
        Func<Task> act = async () => await _sut.CreateRoomAsync(dto, default);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("The Requested Hotel Not found");
    }

    [Fact]
    public async Task DeleteRoomByIdAsync_ShouldDelete_WhenRoomExists()
    {
        // Arrange
        var room = _fixture.Create<Room>();
        var roomId = room.Id;

        _roomRepoMock.Setup(r => r.GetRoomByIdAsync(roomId, default))
            .ReturnsAsync(room);

        // Act
        await _sut.DeleteRoomByIdAsync(roomId, default);

        // Assert
        _roomRepoMock.Verify(r => r.DeleteRoomByIdAsync(roomId, default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteRoomByIdAsync_ShouldThrowNotFoundException_WhenRoomNotFound()
    {
        // Arrange
        var roomId = Guid.NewGuid();

        _roomRepoMock.Setup(r => r.GetRoomByIdAsync(roomId, default))
            .ReturnsAsync((Room)null);

        // Act
        Func<Task> act = async () => await _sut.DeleteRoomByIdAsync(roomId, default);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("The Requested Room Not found");
    }

    [Fact]
    public async Task UpdateRoomAsync_ShouldUpdate_WhenRoomExists()
    {
        // Arrange
        var dto = _fixture.Create<UpdateRoomDto>();
        var room = _fixture.Create<Room>();

        _roomRepoMock.Setup(r => r.GetRoomByIdAsync(dto.Id, default))
            .ReturnsAsync(room);

        // Act
        await _sut.UpdateRoomAsync(dto, default);

        // Assert
        _mapperMock.Verify(m => m.Map(dto, room), Times.Once);
        _roomRepoMock.Verify(r => r.UpdateRoomAsync(room, default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateRoomAsync_ShouldThrowNotFoundException_WhenRoomNotFound()
    {
        // Arrange
        var dto = _fixture.Create<UpdateRoomDto>();

        _roomRepoMock.Setup(r => r.GetRoomByIdAsync(dto.Id, default))
            .ReturnsAsync((Room)null);

        // Act
        Func<Task> act = async () => await _sut.UpdateRoomAsync(dto, default);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("The Requested Room Not found");
    }
}
