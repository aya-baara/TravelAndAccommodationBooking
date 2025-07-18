using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using BookingPlatform.Application.Dtos.Cities;
using BookingPlatform.Application.Services.Queries;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Sieve.Services;
using BookingPlatform.Application.Services.Commands;

namespace BookingPlatform.Tests.Queries;

public class CityQueryServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ICityRepository> _cityRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<CityCommandService>> _loggerMock;
    private readonly Mock<ISieveProcessor> _sieveMock;
    private readonly CityQueryService _service;

    public CityQueryServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());

        _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));

        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _fixture.Customize<DateOnly>(c =>
           c.FromFactory(() => DateOnly.FromDateTime(DateTime.Today.AddYears(-_fixture.Create<int>() % 40 - 20))));

        _cityRepositoryMock = _fixture.Freeze<Mock<ICityRepository>>();
        _mapperMock = _fixture.Freeze<Mock<IMapper>>();
        _loggerMock = _fixture.Freeze<Mock<ILogger<CityCommandService>>>();
        _sieveMock = _fixture.Freeze<Mock<ISieveProcessor>>();

        _service = new CityQueryService(
            _cityRepositoryMock.Object,
            _mapperMock.Object,
            _loggerMock.Object,
            _sieveMock.Object
        );
    }

    [Fact]
    public async Task GetCityByIdAsync_ShouldReturnMappedDto_WhenCityExists()
    {
        // Arrange
        var city = _fixture.Create<City>();
        var cityDto = _fixture.Create<CityResponseDto>();

        _cityRepositoryMock.Setup(r => r.GetCityByIdAsync(city.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(city);
        _mapperMock.Setup(m => m.Map<CityResponseDto>(city))
            .Returns(cityDto);

        // Act
        var result = await _service.GetCityByIdAsync(city.Id, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(cityDto);
    }

    [Fact]
    public async Task GetCityByIdAsync_ShouldThrowNotFoundException_WhenCityNotExists()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        _cityRepositoryMock.Setup(r => r.GetCityByIdAsync(cityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((City?)null);

        // Act
        var act = async () => await _service.GetCityByIdAsync(cityId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("The Requested City Not found");
    }

    [Fact]
    public async Task GetTopVisitedCities_ShouldReturnMappedList()
    {
        // Arrange
        var cities = _fixture.CreateMany<City>(3).ToList();
        var cityDtos = _fixture.CreateMany<CityResponseDto>(3).ToList();

        _cityRepositoryMock.Setup(r => r.GetTopBookedCitiesAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cities);

        _mapperMock.Setup(m => m.Map<List<CityResponseDto>>(cities)).Returns(cityDtos);

        // Act
        var result = await _service.GetTopVisitedCities(3, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(cityDtos);
    }

}
