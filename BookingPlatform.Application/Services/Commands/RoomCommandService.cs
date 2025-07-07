using AutoMapper;
using BookingPlatform.Application.Dtos.Rooms;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace BookingPlatform.Application.Services.Commands;

public class RoomCommandService : IRoomCommandService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<RoomCommandService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public RoomCommandService(IRoomRepository roomRepository
        , IHotelRepository hotelRepository
        , IMapper mapper
        , ILogger<RoomCommandService> logger
        , IUnitOfWork unitOfWork)
    {
        _roomRepository = roomRepository;
        _hotelRepository = hotelRepository;
        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<RoomResponseDto> CreateRoomAsync(CreateRoomDto dto, CancellationToken cancellationToken)
    {
        var hotel = await _hotelRepository.GetHotelByIdAsync(dto.HotelId, cancellationToken);
        if (hotel is null)
        {
            _logger.LogWarning($"Attempted to Add Room to non-existent Hotel with ID {dto.HotelId}");
            throw new NotFoundException("The Requested Hotel Not found");
        }
        var room = _mapper.Map<Room>(dto);
        var created = await _roomRepository.CreateRoomAsync(room, cancellationToken);

        _logger.LogInformation($"Room Created successfully with ID {created.Id}");

        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<RoomResponseDto>(created);
    }

    public async Task DeleteRoomByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetRoomByIdAsync(id, cancellationToken);
        if (room is null)
        {
            _logger.LogWarning($"Attempted to Delete non-existent Room {id}");
            throw new NotFoundException("The Requested Room Not found");
        }
        await _roomRepository.DeleteRoomByIdAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Room Deleted successfully with ID {id}");
    }

    public async Task UpdateRoomAsync(UpdateRoomDto dto, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetRoomByIdAsync(dto.Id, cancellationToken);
        if (room is null)
        {
            _logger.LogWarning($"Attempted to Update non-existent Room {dto.Id}");
            throw new NotFoundException("The Requested Room Not found");
        }
        _mapper.Map(dto, room);
        await _roomRepository.UpdateRoomAsync(room, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Room updated successfully with ID {dto.Id}");
    }
}

