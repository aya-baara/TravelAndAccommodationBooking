using AutoMapper;
using BookingPlatform.Application.Dtos.Discounts;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace BookingPlatform.Application.Services.Queries;

public class DiscountQueryService : IDiscountQueryService
{
    private readonly IDiscountRepository _discountRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<DiscountQueryService> _logger;

    public DiscountQueryService(IDiscountRepository discountRepository
        , IRoomRepository roomRepository
        , IMapper mapper
        , ILogger<DiscountQueryService> logger)
    {
        _discountRepository = discountRepository;
        _roomRepository = roomRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<DiscountResponseDto> GetDiscountByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var discount = await _discountRepository.GetDiscountByIdAsync(id, cancellationToken);
        if (discount is null)
        {
            _logger.LogWarning($"Discount with ID {id} not found");
            throw new NotFoundException("The Requested Discount Not found");
        }

        _logger.LogInformation($"Successfully retrieved Discount with ID {id}");

        return _mapper.Map<DiscountResponseDto>(discount);
    }

    public async Task<List<DiscountResponseDto>> GetDiscountsByRoom(Guid roomId, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetRoomByIdAsync(roomId, cancellationToken);
        if (room is null)
        {
            _logger.LogWarning($"Room with ID {roomId} not found");
            throw new NotFoundException("The Requested Room Not found");

        }
        var discounts =await _discountRepository.GetDiscountByRoomIdAsync(roomId);

        _logger.LogInformation($"Successfully get The Discount for {roomId} Room ");

        return _mapper.Map<List<DiscountResponseDto>>(discounts);
    }
}

