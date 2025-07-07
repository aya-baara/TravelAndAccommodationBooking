using AutoMapper;
using BookingPlatform.Application.Dtos.Images;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Core.Interfaces;
using BookingPlatform.Core.Enums;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Services.Commands;

public class ImageCommandService : IImageCommandService
{
    private readonly IImageRepository _imageRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ImageCommandService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ImageCommandService(IImageRepository imageRepository
        , IHotelRepository hotelRepository
        , IRoomRepository roomRepository
        , IMapper mapper
        , ILogger<ImageCommandService> logger
        , IUnitOfWork unitOfWork)
    {
        _imageRepository = imageRepository;
        _hotelRepository = hotelRepository;
        _roomRepository = roomRepository;
        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<ImageResponseDto> CreateImageAsync(CreateImageDto dto, CancellationToken cancellationToken)
    {
        var hotel = await _hotelRepository.GetHotelByIdAsync(dto.HotelId, cancellationToken);
        if (hotel is null && (dto.Type == ImageType.HotelMain || dto.Type == ImageType.HotelGallery))
        {
            _logger.LogWarning($"Attempted to Add Image to non-existent Hotel with ID {dto.HotelId}");
            throw new NotFoundException("The Requested Hotel Not found");
        }
        var room = await _roomRepository.GetRoomByIdAsync(dto.RoomId, cancellationToken);
        if (room is null && (dto.Type == ImageType.RoomMain || dto.Type == ImageType.RoomGallery))
        {
            _logger.LogWarning($"Attempted to Add Image to non-existent Room with ID {dto.RoomId}");
            throw new NotFoundException("The Requested Room Not found");
        }
        var image = _mapper.Map<Image>(dto);
        var created = await _imageRepository.CreateAsync(image);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Image Created successfully with ID {created.Id}");

        return _mapper.Map<ImageResponseDto>(created);
    }

    public async Task DeleteImageAsync(Guid id, CancellationToken cancellationToken)
    {
        var image = await _imageRepository.GetImageByIdAsync(id, cancellationToken);
        if (image is null)
        {
            _logger.LogWarning($"Attempted to Delete non-existent Image {id}");
            throw new NotFoundException("The Requested Image Not found");
        }
        await _imageRepository.DeleteImageByIdAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Image Deleted successfully with ID {id}");
    }

    public async Task UpdateImageAsync(UpdateImageDto dto, CancellationToken cancellationToken)
    {
        var image = await _imageRepository.GetImageByIdAsync(dto.Id, cancellationToken);
        if (image is null)
        {
            _logger.LogWarning($"Attempted to Delete non-existent Image {dto.Id}");
            throw new NotFoundException("The Requested Image Not found");
        }
        var hotel = await _hotelRepository.GetHotelByIdAsync(dto.HotelId);
        if (hotel is null && (dto.Type == ImageType.HotelMain || dto.Type == ImageType.HotelGallery))
        {
            _logger.LogWarning($"Attempted to Add Image to non-existent Hotel with ID {dto.HotelId}");
            throw new NotFoundException("The Requested Hotel Not found");
        }
        var room = await _roomRepository.GetRoomByIdAsync(dto.RoomId, cancellationToken);
        if (room is null && (dto.Type == ImageType.RoomMain || dto.Type == ImageType.RoomGallery))
        {
            _logger.LogWarning($"Attempted to Add Image to non-existent Room with ID {dto.RoomId}");
            throw new NotFoundException("The Requested Room Not found");
        }
        _mapper.Map(dto, image);
        await _imageRepository.UpdateImageAsync(image, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Image updated successfully with ID {dto.Id}");
    }
}

