using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Repositories;

public interface IRoomRepository
{
<<<<<<< HEAD
    Task<Room> CreateRoomAsync(Room room);
    Task<Room?> GetRoomByIdAsync(Guid roomId);
    Task<PaginatedResult<Room>> GetAvailbleRoomsByHotelIdAsync(Guid hotelId, int page, int size);
    Task<bool> IsRoomAvailbleAsync(Guid roomId);
    Task UpdateRoomAsync(Room room);
    Task DeleteRoomByIdAsync(Guid roomId);
=======
    Task<Room> CreateRoomAsync(Room room, CancellationToken cancellationToken = default);
    Task<Room?> GetRoomByIdAsync(Guid roomId, CancellationToken cancellationToken = default);
    Task<PaginatedResult<Room>> GetAvailbleRoomsByHotelIdAsync(Guid hotelId, int page = 1, int size = 20
        , CancellationToken cancellationToken = default);
    Task<bool> IsRoomAvailbleAsync(Guid roomId, CancellationToken cancellationToken = default);
    Task UpdateRoomAsync(Room room, CancellationToken cancellationToken = default);
    Task DeleteRoomByIdAsync(Guid roomId, CancellationToken cancellationToken = default);
>>>>>>> coreStructure

}

