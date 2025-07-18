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

    public async Task<Image> CreateAsync(Image image, CancellationToken cancellationToken = default)
    {
        var result = await _context.Images.AddAsync(image, cancellationToken);
        return result.Entity;
    }

    public async Task DeleteImageByIdAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        var image = await GetImageByIdAsync(imageId, cancellationToken);
        if (image != null)
        {
            _context.Images.Remove(image);
        }
    }

    public async Task<List<Image>> GetHotelGalleryImagesAsync(Guid hotelId, CancellationToken cancellationToken = default)
    {
        return await _context.Images
             .AsNoTracking()
             .Where(i => i.HotelId == hotelId && i.Type == ImageType.HotelGallery)
             .ToListAsync(cancellationToken);
    }

    public async Task<Image?> GetHotelThumbnailImageAsync(Guid hotelId, CancellationToken cancellationToken = default)
    {
        return await _context.Images
            .FirstOrDefaultAsync(i => i.HotelId == hotelId && i.Type == ImageType.Thumbnail, cancellationToken);
    }

    public async Task<Image?> GetImageByIdAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        return await _context.Images.FirstOrDefaultAsync(i => i.Id == imageId, cancellationToken);
    }

    public async Task<List<Image>> GetRoomGalleryImagesAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        return await _context.Images
            .AsNoTracking()
            .Where(i => i.RoomId == roomId && i.Type == ImageType.RoomGallery)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateImageAsync(Image image, CancellationToken cancellationToken = default)
    {
        _context.Images.Update(image);
    }
}

