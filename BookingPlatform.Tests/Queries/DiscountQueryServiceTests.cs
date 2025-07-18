using AutoFixture;
using AutoMapper;
using BookingPlatform.Application.Dtos.Discounts;
using BookingPlatform.Application.Services.Queries;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookingPlatform.Tests.Queries;

public class DiscountQueryServiceTests
{
    private readonly Fixture _fixture;
    private readonly Mock<IDiscountRepository> _discountRepositoryMock;
    private readonly Mock<IRoomRepository> _roomRepositoryMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<DiscountQueryService>> _loggerMock;
    private readonly DiscountQueryService _service;

    public DiscountQueryServiceTests()
    {
        _fixture = new Fixture();

        _fixture.Behaviors
      .OfType<ThrowingRecursionBehavior>()
      .ToList()
      .ForEach(b => _fixture.Behaviors.Remove(b));

        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _fixture.Customize<DateOnly>(c =>
           c.FromFactory(() => DateOnly.FromDateTime(DateTime.Today.AddYears(-_fixture.Create<int>() % 40 - 20))));

        _discountRepositoryMock = new Mock<IDiscountRepository>();
        _roomRepositoryMock = new Mock<IRoomRepository>();
        _loggerMock = new Mock<ILogger<DiscountQueryService>>();

        // Setup AutoMapper configuration for Discount to DiscountResponseDto mapping
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<BookingPlatform.Core.Entities.Discount, DiscountResponseDto>();
        });
        _mapper = config.CreateMapper();

        _service = new DiscountQueryService(
            _discountRepositoryMock.Object,
            _roomRepositoryMock.Object,
            _mapper,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetDiscountByIdAsync_ReturnsMappedDiscount_WhenFound()
    {
        // Arrange
        var discountEntity = _fixture.Create<BookingPlatform.Core.Entities.Discount>();
        var id = discountEntity.Id;

        _discountRepositoryMock
            .Setup(r => r.GetDiscountByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(discountEntity);

        // Act
        var result = await _service.GetDiscountByIdAsync(id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(discountEntity.Id, result.Id);
        _discountRepositoryMock.Verify(r => r.GetDiscountByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetDiscountByIdAsync_ThrowsNotFoundException_WhenDiscountNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _discountRepositoryMock
            .Setup(r => r.GetDiscountByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BookingPlatform.Core.Entities.Discount?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.GetDiscountByIdAsync(id, CancellationToken.None));

        Assert.Equal("The Requested Discount Not found", ex.Message);
        _discountRepositoryMock.Verify(r => r.GetDiscountByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetDiscountsByRoom_ReturnsMappedDiscounts_WhenRoomFound()
    {
        // Arrange
        var roomId = Guid.NewGuid();

        var roomEntity = _fixture.Create<BookingPlatform.Core.Entities.Room>();
        roomEntity.Id = roomId;

        var discounts = _fixture.CreateMany<BookingPlatform.Core.Entities.Discount>(3).ToList();

        _roomRepositoryMock
            .Setup(r => r.GetRoomByIdAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(roomEntity);

        _discountRepositoryMock
    .Setup(r => r.GetDiscountByRoomIdAsync(roomId, default))
    .ReturnsAsync(discounts);

        // Act
        var result = await _service.GetDiscountsByRoom(roomId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        _roomRepositoryMock.Verify(r => r.GetRoomByIdAsync(roomId, It.IsAny<CancellationToken>()), Times.Once);
        _discountRepositoryMock.Verify(r => r.GetDiscountByRoomIdAsync(roomId,default), Times.Once);
    }

    [Fact]
    public async Task GetDiscountsByRoom_ThrowsNotFoundException_WhenRoomNotFound()
    {
        // Arrange
        var roomId = Guid.NewGuid();

        _roomRepositoryMock
            .Setup(r => r.GetRoomByIdAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BookingPlatform.Core.Entities.Room?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.GetDiscountsByRoom(roomId, CancellationToken.None));

        Assert.Equal("The Requested Room Not found", ex.Message);
        _roomRepositoryMock.Verify(r => r.GetRoomByIdAsync(roomId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
