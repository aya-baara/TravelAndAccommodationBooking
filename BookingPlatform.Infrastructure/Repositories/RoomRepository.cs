using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingPlatform.Infrastructure.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly AppDbContext _context;
    public RoomRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Room> CreateRoomAsync(Room room)
    {
        var result = await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task DeleteRoomByIdAsync(Guid roomId)
    {
        var room = await GetRoomByIdAsync(roomId);
        if(room != null)
        {
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<PaginatedResult<Room>> GetAvailbleRoomsByHotelIdAsync(Guid hotelId, int page, int size)
    {
        var totalCount = await _context.Rooms.Where(r=>r.HotelId==hotelId && r.IsAvailble == true).CountAsync();

        var items = await _context.Rooms
            .Where(r => r.HotelId == hotelId && r.IsAvailble == true)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return new PaginatedResult<Room>(items, totalCount, page, size);
    }

    public async Task<Room?> GetRoomByIdAsync(Guid roomId)
    {
        return await _context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
    }

    public async Task<bool> IsRoomAvailbleAsync(Guid roomId)
    {
        return (await GetRoomByIdAsync(roomId))?.IsAvailble == true;
    }

    public async Task UpdateRoomAsync(Room room)
    {
        _context.Rooms.Update(room);
        await _context.SaveChangesAsync();
    }
}

