using BookingPlatform.Application.Dtos.Rooms;

namespace BookingPlatform.Application.Interfaces.Commands;

public interface IRoomCommandService
{
    Task<RoomResponseDto> CreateRoomAsync(CreateRoomDto dto, CancellationToken cancellationToken);
    Task DeleteRoomByIdAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateRoomAsync(UpdateRoomDto dto, CancellationToken cancellationToken);
}

