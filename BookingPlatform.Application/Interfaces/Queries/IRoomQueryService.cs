using BookingPlatform.Application.Dtos.Rooms;

namespace BookingPlatform.Application.Interfaces.Queries;

public interface IRoomQueryService
{
    Task<RoomResponseDto> GetRoomByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<RoomResponseDto>> GetAvailableRoomsByHotelAsync(Guid hotelId, CancellationToken ct);
    Task<PaginatedResult<RoomManagementDto>> SearchRoomsAsync(RoomAdminSearchRequest request, CancellationToken ct);
}

