using AutoMapper;
using BookingPlatform.Application.Dtos.Reviews;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace BookingPlatform.Application.Services.Commands;

public class ReviewCommandService : IReviewCommandService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ReviewCommandService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ReviewCommandService(IReviewRepository reviewRepository
        , IHotelRepository hotelRepository
        , IUserRepository userRepository
        , IMapper mapper
        , ILogger<ReviewCommandService> logger
        , IUnitOfWork unitOfWork)
    {
        _reviewRepository = reviewRepository;
        _hotelRepository = hotelRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReviewResponseDto> CreateReviewAsync(CreateReviewDto dto, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(dto.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning($"Attempted to Add Review to non-existent User with ID {dto.UserId}");
            throw new NotFoundException("The Requested User Not found");
        }
        var hotel = await _hotelRepository.GetHotelByIdAsync(dto.HotelId, cancellationToken);
        if (hotel is null)
        {
            _logger.LogWarning($"Attempted to Add Review to non-existent Hotel with ID {dto.HotelId}");
            throw new NotFoundException("The Requested Hotel Not found");
        }

        var review = _mapper.Map<Review>(dto);
        var created = await _reviewRepository.CreateReviewAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Review Created successfully with ID {created.Id}");

        return _mapper.Map<ReviewResponseDto>(created);
    }

    public async Task DeleteReview(Guid id, CancellationToken cancellationToken)
    {
        var review =await _reviewRepository.GetReviewByIdAsync(id, cancellationToken);
        if(review is null)
        {
            _logger.LogWarning($"Attempted to Delete non-existent Review {id}");
            throw new NotFoundException("The Requested Review Not found");
        }
        await _reviewRepository.DeleteReviewById(id,cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Review Deleted successfully with ID {id}");
    }

    public async Task UpdateReview(UpdateReviewDto dto, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetReviewByIdAsync(dto.Id, cancellationToken);
        if (review is null)
        {
            _logger.LogWarning($"Attempted to Delete non-existent Review {dto.Id}");
            throw new NotFoundException("The Requested Review Not found");
        }
        var user = await _userRepository.GetUserByIdAsync(dto.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning($"Attempted to Add Review to non-existent User with ID {dto.UserId}");
            throw new NotFoundException("The Requested User Not found");
        }
        var hotel = await _hotelRepository.GetHotelByIdAsync(dto.HotelId, cancellationToken);
        if (hotel is null)
        {
            _logger.LogWarning($"Attempted to Add Review to non-existent Hotel with ID {dto.HotelId}");
            throw new NotFoundException("The Requested Hotel Not found");
        }
        _mapper.Map(dto, review);
        await _reviewRepository.UpdateReviewAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Review updated successfully with ID {dto.Id}");

    }
}

