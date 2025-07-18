﻿using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Repositories;

public interface IRoomRepository
{
    Task<Room> CreateRoomAsync(Room room, CancellationToken cancellationToken = default);
    Task<Room?> GetRoomByIdAsync(Guid roomId, CancellationToken cancellationToken = default);
    Task UpdateRoomAsync(Room room, CancellationToken cancellationToken = default);
    Task DeleteRoomByIdAsync(Guid roomId, CancellationToken cancellationToken = default);
    Task<List<Room>> GetAvailbleRoomsByHotelId(Guid hotelId, CancellationToken cancellationToken = default);
    IQueryable<Room> GetAllAsQueryable();
}

