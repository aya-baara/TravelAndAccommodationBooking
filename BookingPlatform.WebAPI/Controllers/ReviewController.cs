using BookingPlatform.Application.Dtos.Reviews;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingPlatform.WebAPI.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewController : ControllerBase
{
    private readonly IReviewCommandService _reviewCommandService;
    private readonly IReviewQueryService _reviewQueryService;

    public ReviewController(
        IReviewCommandService reviewCommandService,
        IReviewQueryService reviewQueryService)
    {
        _reviewCommandService = reviewCommandService;
        _reviewQueryService = reviewQueryService;
    }

    /// <summary>
    /// Create a new review for a hotel.
    /// </summary>
    /// <param name="dto">Review details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created review.</returns>
    /// <response code="201">Review created successfully.</response>
    /// <response code="404">User or hotel not found.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ReviewResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ReviewResponseDto>> CreateReview([FromBody] CreateReviewDto dto, CancellationToken cancellationToken)
    {
        var result = await _reviewCommandService.CreateReviewAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetReviewById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Get a review by ID.
    /// </summary>
    /// <param name="id">Review ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The review.</returns>
    /// <response code="200">Review found.</response>
    /// <response code="404">Review not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ReviewResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewResponseDto>> GetReviewById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _reviewQueryService.GetReviewByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get reviews for a specific hotel (paginated).
    /// </summary>
    /// <param name="hotelId">Hotel ID.</param>
    /// <param name="page">Page number (default 1).</param>
    /// <param name="size">Page size (default 10).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of reviews for the hotel.</returns>
    /// <response code="200">Reviews retrieved.</response>
    [HttpGet("hotel/{hotelId:guid}")]
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
    /// Update an existing review.
    /// </summary>
    /// <param name="dto">Updated review data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Review updated successfully.</response>
    /// <response code="404">Review, hotel, or user not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpPut]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateReview([FromBody] UpdateReviewDto dto, CancellationToken cancellationToken)
    {
        await _reviewCommandService.UpdateReview(dto, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Delete a review by ID.
    /// </summary>
    /// <param name="id">Review ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Review deleted.</response>
    /// <response code="404">Review not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteReview(Guid id, CancellationToken cancellationToken)
    {
        await _reviewCommandService.DeleteReview(id, cancellationToken);
        return NoContent();
    }
}
