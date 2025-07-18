using AutoMapper;
using BookingPlatform.Application.Dtos.Reviews;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace BookingPlatform.Application.Services.Queries;

public class ReviewQueryService : IReviewQueryService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ReviewQueryService> _logger;

    public ReviewQueryService(IReviewRepository reviewRepository
        , IMapper mapper
        , ILogger<ReviewQueryService> logger)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaginatedResult<ReviewResponseDto>> GetHotelReviews(Guid id, CancellationToken cancellationToken, int page, int size)
    {
        var reviews = await _reviewRepository.GetReviewsByHotelIdAsync(id, page, size, cancellationToken);

        _logger.LogInformation($"Successfully retrieved Reviews with Hotel ID {id}");

        var mappedItems = _mapper.Map<List<ReviewResponseDto>>(reviews.Items);

        return new PaginatedResult<ReviewResponseDto>(mappedItems, reviews.Items.Count(), page, size);
    }
    public async Task<ReviewResponseDto> GetReviewByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetReviewByIdAsync(id, cancellationToken);
        if (review is null)
        {
            _logger.LogWarning($"Review with ID {id} not found");
            throw new NotFoundException("The Requested Review Not found");
        }

        _logger.LogInformation($"Successfully retrieved Review with ID {id}");

        return _mapper.Map<ReviewResponseDto>(review);
    }
}

