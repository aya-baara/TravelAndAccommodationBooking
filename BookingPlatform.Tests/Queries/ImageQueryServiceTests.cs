using AutoMapper;
using BookingPlatform.Application.Dtos.Images;
using BookingPlatform.Application.Services.Queries;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookingPlatform.Tests.Queries;

public class ImageQueryServiceTests : IClassFixture<ImageQueryServiceTests.Fixture>
{
    private readonly Fixture _fixture;

    public ImageQueryServiceTests(Fixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetImageByIdAsync_ShouldReturnImage_WhenFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var imageEntity = new Image { Id = id };
        var imageDto = new ImageResponseDto();

        _fixture.ImageRepoMock
            .Setup(r => r.GetImageByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(imageEntity);

        _fixture.MapperMock
            .Setup(m => m.Map<ImageResponseDto>(imageEntity))
            .Returns(imageDto);

        var service = _fixture.CreateService();

        // Act
        var result = await service.GetImageByIdAsync(id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(imageDto, result);
        _fixture.ImageRepoMock.Verify(r => r.GetImageByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _fixture.MapperMock.Verify(m => m.Map<ImageResponseDto>(imageEntity), Times.Once);
    }

    [Fact]
    public async Task GetImageByIdAsync_ShouldThrowNotFound_WhenImageNotFound()
    {
        // Arrange
        _fixture.ImageRepoMock
            .Setup(r => r.GetImageByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Image?)null);

        var service = _fixture.CreateService();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.GetImageByIdAsync(Guid.NewGuid(), CancellationToken.None));

        Assert.Equal("The Requested Image Not found", ex.Message);
    }

    public class Fixture
    {
        public Mock<IImageRepository> ImageRepoMock { get; } = new();
        public Mock<IMapper> MapperMock { get; } = new();
        public Mock<ILogger<ImageQueryService>> LoggerMock { get; } = new();

        public ImageQueryService CreateService()
        {
            return new ImageQueryService(
                ImageRepoMock.Object,
                MapperMock.Object,
                LoggerMock.Object);
        }
    }
}
