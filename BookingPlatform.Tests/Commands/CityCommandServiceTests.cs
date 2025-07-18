using AutoMapper;
using BookingPlatform.Application.Dtos.Cities;
using BookingPlatform.Application.Services.Commands;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookingPlatform.Tests.Commands;

public class CityCommandServiceTests
{
    private readonly Mock<ICityRepository> _cityRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly IMapper _mapper;
    private readonly ILogger<CityCommandService> _logger = new LoggerFactory().CreateLogger<CityCommandService>();
    private readonly CityCommandService _service;

    public CityCommandServiceTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateCityDto, City>();
            cfg.CreateMap<City, CityResponseDto>();
            cfg.CreateMap<UpdateCityDto, City>();
        });
        _mapper = config.CreateMapper();

        _service = new CityCommandService(
            _cityRepoMock.Object,
            _mapper,
            _logger,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task CreateCityAsync_ShouldCreateCity()
    {
        // Arrange
        var dto = new CreateCityDto { Name = "Test City" };
        var city = new City { Id = Guid.NewGuid(), Name = dto.Name };

        _cityRepoMock
            .Setup(r => r.CreateCityAsync(It.IsAny<City>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(city);

        // Act
        var result = await _service.CreateCityAsync(dto, CancellationToken.None);

        // Assert
        Assert.Equal(city.Id, result.Id);
        Assert.Equal(city.Name, result.Name);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteCityAsync_ShouldDeleteCity_WhenExists()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        var city = new City { Id = cityId, Name = "Some City" };

        _cityRepoMock.Setup(r => r.GetCityByIdAsync(cityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(city);

        // Act
        await _service.DeleteCityAsync(cityId, CancellationToken.None);

        // Assert
        _cityRepoMock.Verify(r => r.DeleteCityAsync(cityId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteCityAsync_ShouldThrowNotFound_WhenCityDoesNotExist()
    {
        // Arrange
        var cityId = Guid.NewGuid();

        _cityRepoMock.Setup(r => r.GetCityByIdAsync(cityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((City)null!);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.DeleteCityAsync(cityId, CancellationToken.None));

        Assert.Equal("The Requested City Not found", ex.Message);
    }

    [Fact]
    public async Task UpdateCityAsync_ShouldUpdateCity_WhenExists()
    {
        // Arrange
        var dto = new UpdateCityDto { Id = Guid.NewGuid(), Name = "Updated City" };
        var city = new City { Id = dto.Id, Name = "Old City" };

        _cityRepoMock.Setup(r => r.GetCityByIdAsync(dto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(city);

        // Act
        await _service.UpdateCityAsync(dto, CancellationToken.None);

        // Assert
        _cityRepoMock.Verify(r => r.UpdateCityAsync(city,default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        Assert.Equal(dto.Name, city.Name); // Mapper applied
    }

    [Fact]
    public async Task UpdateCityAsync_ShouldThrowNotFound_WhenCityDoesNotExist()
    {
        // Arrange
        var dto = new UpdateCityDto { Id = Guid.NewGuid(), Name = "Any City" };

        _cityRepoMock.Setup(r => r.GetCityByIdAsync(dto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((City)null!);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.UpdateCityAsync(dto, CancellationToken.None));

        Assert.Equal("The Requested City Not found", ex.Message);
    }
}
