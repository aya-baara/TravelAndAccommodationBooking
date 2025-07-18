using AutoMapper;
using BookingPlatform.Application.Dtos.Discounts;
using BookingPlatform.Application.Services.Commands;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookingPlatform.Tests.Commands;

public class DiscountCommandServiceTests
{
    private readonly Mock<IDiscountRepository> _discountRepoMock = new();
    private readonly Mock<IRoomRepository> _roomRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IMapper _mapper;
    private readonly ILogger<DiscountCommandService> _logger = new LoggerFactory().CreateLogger<DiscountCommandService>();

    private readonly DiscountCommandService _service;

    public DiscountCommandServiceTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateDiscountDto, Discount>();
            cfg.CreateMap<Discount, DiscountResponseDto>();
            cfg.CreateMap<UpdateDiscountDto, Discount>();
        });
        _mapper = config.CreateMapper();

        _service = new DiscountCommandService(
            _discountRepoMock.Object,
            _roomRepoMock.Object,
            _mapper,
            _logger,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task CreateDiscountAsync_ShouldCreateDiscount_WhenRoomExists()
    {
        // Arrange
        var dto = new CreateDiscountDto
        {
            RoomId = Guid.NewGuid(),
            Percentage = 10,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(5)
        };

        var room = new Room { Id = dto.RoomId };
        var discount = new Discount { Id = Guid.NewGuid(), RoomId = dto.RoomId };

        _roomRepoMock.Setup(r => r.GetRoomByIdAsync(dto.RoomId, default)).ReturnsAsync(room);
        _discountRepoMock.Setup(d => d.CreateDiscountAsync(It.IsAny<Discount>(), default)).ReturnsAsync(discount);

        // Act
        var result = await _service.CreateDiscountAsync(dto, default);

        // Assert
        Assert.Equal(discount.Id, result.Id);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateDiscountAsync_ShouldThrowNotFound_WhenRoomDoesNotExist()
    {
        // Arrange
        var dto = new CreateDiscountDto
        {
            RoomId = Guid.NewGuid(),
            Percentage = 10,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(5)
        };

        _roomRepoMock.Setup(r => r.GetRoomByIdAsync(dto.RoomId, default)).ReturnsAsync((Room)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateDiscountAsync(dto, default));
    }

    [Fact]
    public async Task DeleteDiscountAsync_ShouldDeleteDiscount_WhenExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var discount = new Discount { Id = id };

        _discountRepoMock.Setup(d => d.GetDiscountByIdAsync(id, default)).ReturnsAsync(discount);

        // Act
        await _service.DeleteDiscountAsync(id, default);

        // Assert
        _discountRepoMock.Verify(d => d.DeleteDiscountByIdAsync(id, default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteDiscountAsync_ShouldThrowNotFound_WhenDiscountDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _discountRepoMock.Setup(d => d.GetDiscountByIdAsync(id, default)).ReturnsAsync((Discount)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteDiscountAsync(id, default));
    }

    [Fact]
    public async Task UpdateDiscountAsync_ShouldUpdateDiscount_WhenExists()
    {
        // Arrange
        var dto = new UpdateDiscountDto
        {
            Id = Guid.NewGuid(),
            Percentage = 15,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(2)
        };

        var discount = new Discount { Id = dto.Id };

        _discountRepoMock.Setup(d => d.GetDiscountByIdAsync(dto.Id, default)).ReturnsAsync(discount);

        // Act
        await _service.UpdateDiscountAsync(dto, default);

        // Assert
        _discountRepoMock.Verify(d => d.UpdateDiscountAsync(discount, default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateDiscountAsync_ShouldThrowNotFound_WhenDiscountDoesNotExist()
    {
        // Arrange
        var dto = new UpdateDiscountDto
        {
            Id = Guid.NewGuid(),
            Percentage = 15,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(2)
        };

        _discountRepoMock.Setup(d => d.GetDiscountByIdAsync(dto.Id, default)).ReturnsAsync((Discount)null!);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateDiscountAsync(dto, default));
    }
}
