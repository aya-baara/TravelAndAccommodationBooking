using Xunit;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using BookingPlatform.Application.Services.Commands;
using BookingPlatform.Application.Dtos.Images;
using BookingPlatform.Core.Enums;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces;
using BookingPlatform.Core.Interfaces.Repositories;

namespace BookingPlatform.Tests.Commands;

public class ImageCommandServiceTests
{
    private readonly Mock<IImageRepository> _imageRepoMock = new();
    private readonly Mock<IHotelRepository> _hotelRepoMock = new();
    private readonly Mock<IRoomRepository> _roomRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<ImageCommandService>> _loggerMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    private readonly ImageCommandService _service;

    public ImageCommandServiceTests()
    {
        _service = new ImageCommandService(
            _imageRepoMock.Object,
            _hotelRepoMock.Object,
            _roomRepoMock.Object,
            _mapperMock.Object,
            _loggerMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateImageAsync_WithValidHotelImage_ShouldCreateAndReturnImage()
    {
        // Arrange
        var dto = new CreateImageDto
        {
            HotelId = Guid.NewGuid(),
            Type = ImageType.HotelMain
        };

        var hotel = new Hotel { Id = dto.HotelId };

        _hotelRepoMock.Setup(r => r.GetHotelByIdAsync(dto.HotelId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(hotel);

        var image = new Image { Id = Guid.NewGuid() };
        _mapperMock.Setup(m => m.Map<Image>(dto)).Returns(image);
        _imageRepoMock.Setup(r => r.CreateAsync(image,default)).ReturnsAsync(image);
        _mapperMock.Setup(m => m.Map<ImageResponseDto>(image)).Returns(new ImageResponseDto { Id = image.Id });

        // Act
        var result = await _service.CreateImageAsync(dto, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(image.Id, result.Id);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateImageAsync_HotelNotFound_ThrowsNotFoundException()
    {
        var dto = new CreateImageDto
        {
            HotelId = Guid.NewGuid(),
            Type = ImageType.HotelMain
        };

        _hotelRepoMock.Setup(r => r.GetHotelByIdAsync(dto.HotelId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((Hotel)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.CreateImageAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteImageAsync_ImageExists_ShouldDelete()
    {
        var imageId = Guid.NewGuid();
        var image = new Image { Id = imageId };

        _imageRepoMock.Setup(r => r.GetImageByIdAsync(imageId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(image);

        // Act
        await _service.DeleteImageAsync(imageId, CancellationToken.None);

        // Assert
        _imageRepoMock.Verify(r => r.DeleteImageByIdAsync(imageId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteImageAsync_ImageNotFound_ThrowsNotFoundException()
    {
        var imageId = Guid.NewGuid();

        _imageRepoMock.Setup(r => r.GetImageByIdAsync(imageId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((Image)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.DeleteImageAsync(imageId, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateImageAsync_WithValidRoomImage_ShouldUpdate()
    {
        var imageId = Guid.NewGuid();
        var dto = new UpdateImageDto
        {
            Id = imageId,
            RoomId = Guid.NewGuid(),
            Type = ImageType.RoomGallery
        };

        var image = new Image { Id = imageId };
        var room = new Room { Id = dto.RoomId };

        _imageRepoMock.Setup(r => r.GetImageByIdAsync(imageId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(image);
        _roomRepoMock.Setup(r => r.GetRoomByIdAsync(dto.RoomId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(room);

        // Act
        await _service.UpdateImageAsync(dto, CancellationToken.None);

        // Assert
        _mapperMock.Verify(m => m.Map(dto, image), Times.Once);
        _imageRepoMock.Verify(r => r.UpdateImageAsync(image, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateImageAsync_ImageNotFound_ThrowsNotFoundException()
    {
        var dto = new UpdateImageDto { Id = Guid.NewGuid() };

        _imageRepoMock.Setup(r => r.GetImageByIdAsync(dto.Id, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((Image)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.UpdateImageAsync(dto, CancellationToken.None));
    }
}
