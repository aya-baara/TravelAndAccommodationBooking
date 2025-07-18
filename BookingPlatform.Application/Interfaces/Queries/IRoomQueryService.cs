using BookingPlatform.Application.Dtos.Rooms;
using Sieve.Models;

namespace BookingPlatform.Application.Interfaces.Queries;

public interface IRoomQueryService
{
    Task<RoomResponseDto> GetRoomByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<RoomResponseDto>> GetAvailableRoomsByHotelAsync(Guid hotelId, CancellationToken ct);
    Task<PaginatedResult<RoomManagementDto>> SearchRoomsAsync(SieveModel request, CancellationToken ct);
}

