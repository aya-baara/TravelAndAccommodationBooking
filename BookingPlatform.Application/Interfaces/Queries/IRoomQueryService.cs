using BookingPlatform.Application.Dtos.Rooms;

namespace BookingPlatform.Application.Interfaces.Queries;

public interface IRoomQueryService
{
    Task<RoomResponseDto> GetRoomByIdAsync(Guid id, CancellationToken cancellationToken);
}

