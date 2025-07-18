using AutoMapper;
using BookingPlatform.Application.Dtos.Rooms;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Sieve.Services;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;


namespace BookingPlatform.Application.Services.Queries;

public class RoomQueryService : IRoomQueryService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<RoomQueryService> _logger;
    private readonly ISieveProcessor _sieve;


    public RoomQueryService(IRoomRepository roomRepository
        , IMapper mapper
        , ILogger<RoomQueryService> logger
        , ISieveProcessor sieveProcessor)
    {
        _roomRepository = roomRepository;
        _mapper = mapper;
        _logger = logger;
        _sieve = sieveProcessor;
    }

    public async Task<RoomResponseDto> GetRoomByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetRoomByIdAsync(id, cancellationToken);
        if (room is null)
        {
            _logger.LogWarning($"Room with ID {id} not found");
            throw new NotFoundException("The Requested Room Not found");
        }

        _logger.LogInformation($"Successfully retrieved Room with ID {id}");

        return _mapper.Map<RoomResponseDto>(room);

    }
    public async Task<List<RoomResponseDto>> GetAvailableRoomsByHotelAsync(Guid hotelId, CancellationToken ct)
    {

        _logger.LogInformation("Fetching available rooms for Hotel ID {HotelId}", hotelId);

        var rooms = await _roomRepository.GetAvailbleRoomsByHotelId(hotelId, ct);

        _logger.LogInformation("Found {RoomCount} available rooms for Hotel ID {HotelId}", rooms.Count, hotelId);

        return _mapper.Map<List<RoomResponseDto>>(rooms);
    }
    public async Task<PaginatedResult<RoomManagementDto>> SearchRoomsAsync(SieveModel request, CancellationToken ct)
    {
        _logger.LogInformation("Admin room search initiated. Page: {Page}, PageSize: {PageSize}",
            request.Page, request.PageSize);

        var query = _roomRepository.GetAllAsQueryable()
            .Select(r => new RoomManagementDto
            {
                Id = r.Id,
                IsAvailable = r.Bookings.All(b => b.CheckOut <= DateTime.Today),
                AdultCapacity = r.AdultCapacity,
                ChildrenCapacity = r.ChildrenCapacity,
                CreatedAt = r.CreatedAt,
                ModifiedAt = r.ModifiedAt
            });

        _logger.LogInformation("Applying Sieve filters to room admin search...");

        var filtered = _sieve.Apply(request, query);

        var total = await filtered.CountAsync(ct);
        var data = await filtered.ToListAsync(ct);

        _logger.LogInformation("Admin room search completed. Total matched rooms: {Total}", total);

        return new PaginatedResult<RoomManagementDto>(data, total, request.Page ?? 1, request.PageSize ?? 10);
    }
}

