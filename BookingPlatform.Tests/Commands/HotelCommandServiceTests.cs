using AutoMapper;
using BookingPlatform.Application.Dtos.Hotels;
using BookingPlatform.Application.Services.Commands;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookingPlatform.Tests.Commands;

public class HotelCommandServiceTests
{
    private readonly Mock<IHotelRepository> _hotelRepoMock = new();
    private readonly Mock<ICityRepository> _cityRepoMock = new();
    private readonly Mock<IOwnerRepository> _ownerRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<HotelCommandService>> _loggerMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly HotelCommandService _service;

    public HotelCommandServiceTests()
    {
        _service = new HotelCommandService(
            _hotelRepoMock.Object,
            _cityRepoMock.Object,
            _ownerRepoMock.Object,
            _mapperMock.Object,
            _loggerMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task CreateHotelAsync_ShouldReturnHotelResponseDto_WhenValid()
    {
        // Arrange
        var dto = new CreateHotelDto { CityId = Guid.NewGuid(), OwnerId = Guid.NewGuid(), Name = "Test Hotel" };
        var hotelEntity = new Hotel { Id = Guid.NewGuid(), Name = dto.Name };
        var responseDto = new HotelResponseDto { Id = hotelEntity.Id, Name = hotelEntity.Name };

        _mapperMock.Setup(m => m.Map<Hotel>(dto)).Returns(hotelEntity);
        _cityRepoMock.Setup(c => c.GetCityByIdAsync(dto.CityId,default)).ReturnsAsync(new City());
        _ownerRepoMock.Setup(o => o.GetOwnerByIdAsync(dto.OwnerId,default)).ReturnsAsync(new Owner());
        _hotelRepoMock.Setup(r => r.CreateHotelAsync(hotelEntity, default)).ReturnsAsync(hotelEntity);
        _mapperMock.Setup(m => m.Map<HotelResponseDto>(hotelEntity)).Returns(responseDto);

        // Act
        var result = await _service.CreateHotelAsync(dto, default);

        // Assert
        Assert.Equal(dto.Name, result.Name);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateHotelAsync_ShouldThrowNotFoundException_WhenCityNotFound()
    {
        // Arrange
        var dto = new CreateHotelDto { CityId = Guid.NewGuid(), OwnerId = Guid.NewGuid() };
        _cityRepoMock.Setup(c => c.GetCityByIdAsync(dto.CityId,default)).ReturnsAsync((City?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateHotelAsync(dto, default));
    }

    [Fact]
    public async Task CreateHotelAsync_ShouldThrowNotFoundException_WhenOwnerNotFound()
    {
        // Arrange
        var dto = new CreateHotelDto { CityId = Guid.NewGuid(), OwnerId = Guid.NewGuid() };
        _cityRepoMock.Setup(c => c.GetCityByIdAsync(dto.CityId, default)).ReturnsAsync(new City());
        _ownerRepoMock.Setup(o => o.GetOwnerByIdAsync(dto.OwnerId, default)).ReturnsAsync((Owner?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateHotelAsync(dto, default));
    }

    [Fact]
    public async Task DeleteHotelAsync_ShouldSucceed_WhenHotelExists()
    {
        var id = Guid.NewGuid();
        _hotelRepoMock.Setup(r => r.GetHotelByIdAsync(id, default)).ReturnsAsync(new Hotel());

        // Act
        await _service.DeleteHotelAsync(id, default);

        // Assert
        _hotelRepoMock.Verify(r => r.DeleteHotelByIdAsync(id, default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteHotelAsync_ShouldThrowNotFoundException_WhenHotelNotFound()
    {
        var id = Guid.NewGuid();
        _hotelRepoMock.Setup(r => r.GetHotelByIdAsync(id, default)).ReturnsAsync((Hotel?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteHotelAsync(id, default));
    }

    [Fact]
    public async Task UpdateHotelAsync_ShouldUpdate_WhenHotelExists()
    {
        var dto = new UpdateHotelDto { Id = Guid.NewGuid(), Name = "Updated Hotel" };
        var hotel = new Hotel { Id = dto.Id, Name = "Old Name" };

        _hotelRepoMock.Setup(r => r.GetHotelByIdAsync(dto.Id, default)).ReturnsAsync(hotel);

        // Act
        await _service.UpdateHotelAsync(dto, default);

        // Assert
        _mapperMock.Verify(m => m.Map(dto, hotel), Times.Once);
        _hotelRepoMock.Verify(r => r.UpdateHotelAsync(hotel, default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateHotelAsync_ShouldThrowNotFoundException_WhenHotelNotFound()
    {
        var dto = new UpdateHotelDto { Id = Guid.NewGuid() };
        _hotelRepoMock.Setup(r => r.GetHotelByIdAsync(dto.Id, default)).ReturnsAsync((Hotel?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateHotelAsync(dto, default));
    }
}
