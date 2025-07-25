﻿using BookingPlatform.Core.Entities;
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

    public async Task<Room> CreateRoomAsync(Room room, CancellationToken cancellationToken = default)
    {
        var result = await _context.Rooms.AddAsync(room, cancellationToken);
        return result.Entity;
    }

    public async Task DeleteRoomByIdAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        var room = await GetRoomByIdAsync(roomId, cancellationToken);
        if (room != null)
        {
            _context.Rooms.Remove(room);
        }
    }

    public async Task<List<Room>> GetAvailbleRoomsByHotelId(Guid hotelId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;

        return await _context.Rooms
            .Include(r => r.Bookings)
            .Include(r => r.Images)
            .Where(r => r.HotelId == hotelId)
            .Where(r => !r.Bookings.Any(b =>
                b.CheckIn <= today && b.CheckOut > today))
            .ToListAsync(cancellationToken);
    }

    public async Task<Room?> GetRoomByIdAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        return await _context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId, cancellationToken);
    }

    public async Task UpdateRoomAsync(Room room, CancellationToken cancellationToken = default)
    {
        _context.Rooms.Update(room);
    }
    public IQueryable<Room> GetAllAsQueryable()
    {
        return _context.Rooms
            .Include(r => r.Bookings); 
    }
}