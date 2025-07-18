using AutoFixture;
using AutoMapper;
using BookingPlatform.Application.Dtos.Owners;
using BookingPlatform.Application.Services.Queries;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookingPlatform.Tests.Queries;

public class OwnerQueryServiceTests
{
    private readonly Fixture _fixture;
    private readonly Mock<IOwnerRepository> _ownerRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<OwnerQueryService>> _loggerMock;
    private readonly OwnerQueryService _service;

    public OwnerQueryServiceTests()
    {
        _fixture = new Fixture();

        _fixture.Behaviors
      .OfType<ThrowingRecursionBehavior>()
      .ToList()
      .ForEach(b => _fixture.Behaviors.Remove(b));

        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _fixture.Customize<DateOnly>(c =>
           c.FromFactory(() => DateOnly.FromDateTime(DateTime.Today.AddYears(-_fixture.Create<int>() % 40 - 20))));
        _ownerRepositoryMock = new Mock<IOwnerRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<OwnerQueryService>>();

        _service = new OwnerQueryService(
            _ownerRepositoryMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetOwnerByIdAsync_WhenOwnerExists_ReturnsMappedDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var owner = _fixture.Create<Owner>();
        var dto = _fixture.Create<OwnerResponseDto>();
        var cancellationToken = CancellationToken.None;

        _ownerRepositoryMock
            .Setup(repo => repo.GetOwnerByIdAsync(id, cancellationToken))
            .ReturnsAsync(owner);

        _mapperMock
            .Setup(m => m.Map<OwnerResponseDto>(owner))
            .Returns(dto);

        // Act
        var result = await _service.GetOwnerByIdAsync(id, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto, result);
        _ownerRepositoryMock.Verify(r => r.GetOwnerByIdAsync(id, cancellationToken), Times.Once);
        _mapperMock.Verify(m => m.Map<OwnerResponseDto>(owner), Times.Once);
    }

    [Fact]
    public async Task GetOwnerByIdAsync_WhenOwnerDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _ownerRepositoryMock
            .Setup(repo => repo.GetOwnerByIdAsync(id, cancellationToken))
            .ReturnsAsync((Owner?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.GetOwnerByIdAsync(id, cancellationToken)
        );

        Assert.Equal("The Requested Owner Not found", exception.Message);
        _loggerMock.Verify(l => l.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("not found")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
}
