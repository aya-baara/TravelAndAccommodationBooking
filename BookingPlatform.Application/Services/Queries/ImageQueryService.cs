using AutoMapper;
using BookingPlatform.Application.Dtos.Images;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace BookingPlatform.Application.Services.Queries;

public class ImageQueryService : IImageQueryService
{
    private readonly IImageRepository _imageRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ImageQueryService> _logger;

    public ImageQueryService(IImageRepository imageRepository
        , IMapper mapper
        , ILogger<ImageQueryService> logger)
    {
        _imageRepository = imageRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ImageResponseDto> GetImageByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var image = await _imageRepository.GetImageByIdAsync(id, cancellationToken);
        if (image is null)
        {
            _logger.LogWarning($"Image with ID {id} not found");
            throw new NotFoundException("The Requested Image Not found");
        }

        _logger.LogInformation($"Successfully retrieved Image with ID {id}");

        return _mapper.Map<ImageResponseDto>(image);
    }
}

