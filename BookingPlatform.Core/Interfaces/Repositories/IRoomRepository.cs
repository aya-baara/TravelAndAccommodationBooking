using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Repositories;

public interface IRoomRepository
{
    Task<Room> CreateRoomAsync(Room room);
    Task<Room?> GetRoomByIdAsync(Guid roomId);
    Task<PaginatedResult<Room>> GetAvailbleRoomsByHotelIdAsync(Guid hotelId, int page, int size);
    Task<bool> IsRoomAvailbleAsync(Guid roomId);
    Task UpdateRoomAsync(Room room);
    Task DeleteRoomByIdAsync(Guid roomId);

}

