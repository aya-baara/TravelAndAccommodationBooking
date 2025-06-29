using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Repositories;

public interface IImageRepository
{
<<<<<<< HEAD
    Task<Image> CreateAsync(Image image);
    Task<Image?> GetImageByIdAsync(Guid imageId);
    Task<List<Image>> GetHotelGalleryImagesAsync(Guid hotelId);
    Task<Image?> GetHotelThumbnailImageAsync(Guid hotelId);
    Task<List<Image>> GetRoomGalleryImagesAsync(Guid roomId);
    Task UpdateImageAsync(Image image);
    Task DeleteImageByIdAsync(Guid imageId);
=======
    Task<Image> CreateAsync(Image image, CancellationToken cancellationToken = default);
    Task<Image?> GetImageByIdAsync(Guid imageId, CancellationToken cancellationToken = default);
    Task<List<Image>> GetHotelGalleryImagesAsync(Guid hotelId, CancellationToken cancellationToken = default);
    Task<Image?> GetHotelThumbnailImageAsync(Guid hotelId, CancellationToken cancellationToken = default);
    Task<List<Image>> GetRoomGalleryImagesAsync(Guid roomId, CancellationToken cancellationToken = default);
    Task UpdateImageAsync(Image image, CancellationToken cancellationToken = default);
    Task DeleteImageByIdAsync(Guid imageId, CancellationToken cancellationToken = default);
>>>>>>> coreStructure

}

