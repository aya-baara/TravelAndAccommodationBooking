using AutoMapper;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Core.Interfaces;
using Microsoft.Extensions.Logging;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Application.Dtos.Discounts;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Services.Commands;

public class DiscountCommandService : IDiscountCommandService
{
    private readonly IDiscountRepository _discountRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<DiscountCommandService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DiscountCommandService(IDiscountRepository discountRepository
        , IRoomRepository roomRepository
        , IMapper mapper
        , ILogger<DiscountCommandService> logger
        , IUnitOfWork unitOfWork)
    {
        _discountRepository = discountRepository;
        _roomRepository = roomRepository;
        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<DiscountResponseDto> CreateDiscountAsync(CreateDiscountDto dto, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetRoomByIdAsync(dto.RoomId, cancellationToken);
        if (room is null)
        {
            _logger.LogWarning($"Attempted to Add Discount to non-existent Room with ID {dto.RoomId}");
            throw new NotFoundException("The Requested Room Not found");
        }
        var discount = _mapper.Map<Discount>(dto);
        var created = await _discountRepository.CreateDiscountAsync(discount, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Discount Created successfully with ID {created.Id}");

        return _mapper.Map<DiscountResponseDto>(created);
    }

    public async Task DeleteDiscountAsync(Guid id, CancellationToken cancellationToken)
    {
        var discount = await _discountRepository.GetDiscountByIdAsync(id);
        if (discount == null)
        {
            _logger.LogWarning($"Attempted to delete non-existent discount with ID {id}");
            throw new NotFoundException("The Requested Discount Not found");
        }
        await _discountRepository.DeleteDiscountByIdAsync(id, cancellationToken);

        _logger.LogInformation($"Discount Deleted successfully with ID {id}");

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateDiscountAsync(UpdateDiscountDto dto, CancellationToken cancellationToken)
    {
        var discount = await _discountRepository.GetDiscountByIdAsync(dto.Id, cancellationToken);
        if (discount == null)
        {
            _logger.LogWarning($"Attempted to update non-existent discount with ID {dto.Id}");
            throw new NotFoundException("The Requested Discount Not found");
        }
        _mapper.Map(dto, discount);
        await _discountRepository.UpdateDiscountAsync(discount, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Discount updated successfully with ID {dto.Id}");
    }
}

