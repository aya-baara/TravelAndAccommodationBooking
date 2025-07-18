using AutoMapper;
using BookingPlatform.Application.Dtos.Reviews;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.WebAPI.Dtos.Reviews;
using BookingPlatform.WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingPlatform.WebAPI.Controllers;

[ApiController]
[Route("api/hotels/{hotelId:guid}/reviews")]
public class ReviewController : ControllerBase
{
    private readonly IReviewCommandService _reviewCommandService;
    private readonly IReviewQueryService _reviewQueryService;
    private readonly IMapper _mapper;

    public ReviewController(
        IReviewCommandService reviewCommandService,
        IReviewQueryService reviewQueryService,
        IMapper mapper)
    {
        _reviewCommandService = reviewCommandService;
        _reviewQueryService = reviewQueryService;
        _mapper = mapper;
    }

    /// <summary>
    /// Create a new review for a specific hotel.
    /// </summary>
    /// <param name="hotelId">Hotel ID from route.</param>
    /// <param name="request">Review request body.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created review.</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ReviewResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ReviewResponseDto>> CreateReview(
        Guid hotelId,
        [FromBody] CreateReviewRequestDto request,
        CancellationToken cancellationToken)
    {
        var dto = _mapper.Map<CreateReviewDto>(request);
        dto.HotelId = hotelId;
        dto.UserId = User.GetUserId();

        var result = await _reviewCommandService.CreateReviewAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetReviewById), new { hotelId, reviewId = result.Id }, result);
    }

    /// <summary>
    /// Get a specific review by ID.
    /// </summary>
    /// <param name="hotelId">Hotel ID.</param>
    /// <param name="reviewId">Review ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The review details.</returns>
    [HttpGet("{reviewId:guid}")]
    [ProducesResponseType(typeof(ReviewResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewResponseDto>> GetReviewById(
        Guid hotelId,
        Guid reviewId,
        CancellationToken cancellationToken)
    {
        var result = await _reviewQueryService.GetReviewByIdAsync(reviewId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get all reviews for a specific hotel (paginated).
    /// </summary>
    /// <param name="hotelId">Hotel ID.</param>
    /// <param name="page">Page number (default = 1).</param>
    /// <param name="size">Page size (default = 10).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<ReviewResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ReviewResponseDto>>> GetHotelReviews(
        Guid hotelId,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _reviewQueryService.GetHotelReviews(hotelId, cancellationToken, page, size);
        return Ok(result);
    }

    /// <summary>
    /// Update a review for a specific hotel.
    /// </summary>
    /// <param name="hotelId">Hotel ID.</param>
    /// <param name="reviewId">Review ID.</param>
    /// <param name="request">Updated review data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPut("{reviewId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateReview(
        Guid hotelId,
        Guid reviewId,
        [FromBody] UpdateReviewRequestDto request,
        CancellationToken cancellationToken)
    {
        var dto = _mapper.Map<UpdateReviewDto>(request);
        dto.Id = reviewId;
        dto.HotelId = hotelId;
        dto.UserId = User.GetUserId();

        await _reviewCommandService.UpdateReview(dto, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Delete a review by ID for a specific hotel.
    /// </summary>
    /// <param name="hotelId">Hotel ID</param>
    /// <param name="reviewId">Review ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpDelete("{reviewId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteReview(
        Guid hotelId,
        Guid reviewId,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _reviewCommandService.DeleteReview(reviewId, userId, cancellationToken);
        return NoContent();
    }
}
