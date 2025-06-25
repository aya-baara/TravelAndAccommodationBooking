using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Enums;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingPlatform.Infrastructure.Repositories;

public class ImageRepository : IImageRepository
{
    private readonly AppDbContext _context;

    public ImageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(Image image)
    {
        _context.Images.Add(image);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteImageByIdAsync(Guid imageId)
    {
        var image = await GetImageByIdAsync(imageId);
        if (image != null)
        {
            _context.Images.Remove(image);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Image>> GetHotelGalleryImagesAsync(Guid hotelId)
    {
       return await _context.Images
            .AsNoTracking()
            .Where(i => i.HotelId == hotelId && i.Type == ImageType.HotelGallery)
            .ToListAsync();
    }

    public async Task<Image?> GetHotelThumbnailImageAsync(Guid hotelId)
    {
        return await _context.Images
            .FirstOrDefaultAsync(i => i.HotelId == hotelId && i.Type == ImageType.Thumbnail);
    }

    public async Task<Image?> GetImageByIdAsync(Guid imageId)
    {
        return await  _context.Images.FirstOrDefaultAsync(i => i.Id == imageId);
    }

    public async Task<List<Image>> GetRoomGalleryImagesAsync(Guid roomId)
    {
        return await _context.Images
            .AsNoTracking()
            .Where(i => i.RoomId == roomId && i.Type == ImageType.RoomGallery)
            .ToListAsync();
    }

    public async Task UpdateImageAsync(Image image)
    {
        _context.Images.Update(image);
        await _context.SaveChangesAsync();
    }
}

