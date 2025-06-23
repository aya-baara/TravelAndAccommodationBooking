using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Repositories;

public interface IImageRepository
{
    Task CreateAsync(Image image);
    Task<Image?> GetImageByIdAsync(Guid imageId);
    Task<List<Image>?> GetHotelImagesAsync(Guid hotelId);
    Task<List<Image>?> GetRoomImagesAsync(Guid roomId);
    Task UpdateImageAsync(Image image);
    Task DeleteImageByIdAsync(Guid imageId);

}

