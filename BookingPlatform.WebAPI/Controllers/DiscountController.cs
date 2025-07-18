using BookingPlatform.Application.Dtos.Discounts;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingPlatform.WebAPI.Controllers;

[ApiController]
[Route("api/discounts")]
public class DiscountController : ControllerBase
{
    private readonly IDiscountCommandService _discountCommandService;
    private readonly IDiscountQueryService _discountQueryService;

    public DiscountController(
        IDiscountCommandService discountCommandService,
        IDiscountQueryService discountQueryService)
    {
        _discountCommandService = discountCommandService;
        _discountQueryService = discountQueryService;
    }

    /// <summary>
    /// Create a new discount for a room.
    /// </summary>
    /// <param name="dto">Discount data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created discount.</returns>
    /// <response code="201">Discount created successfully.</response>
    /// <response code="404">Room not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpPost]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(typeof(DiscountResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<DiscountResponseDto>> CreateDiscount([FromBody] CreateDiscountDto dto, CancellationToken cancellationToken)
    {
        var created = await _discountCommandService.CreateDiscountAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetDiscountById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Get discount by ID.
    /// </summary>
    /// <param name="id">Discount ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Discount details.</returns>
    /// <response code="200">Discount found.</response>
    /// <response code="404">Discount not found.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(DiscountResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<DiscountResponseDto>> GetDiscountById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _discountQueryService.GetDiscountByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get all discounts for a specific room.
    /// </summary>
    /// <param name="roomId">Room ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of discounts.</returns>
    /// <response code="200">Discounts retrieved successfully.</response>
    /// <response code="404">Room not found.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("room/{roomId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(List<DiscountResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<DiscountResponseDto>>> GetDiscountsByRoom(Guid roomId, CancellationToken cancellationToken)
    {
        var result = await _discountQueryService.GetDiscountsByRoom(roomId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Update an existing discount.
    /// </summary>
    /// <param name="dto">Updated discount data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Discount updated successfully.</response>
    /// <response code="404">Discount not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpPut]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateDiscount([FromBody] UpdateDiscountDto dto, CancellationToken cancellationToken)
    {
        await _discountCommandService.UpdateDiscountAsync(dto, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Delete a discount by ID.
    /// </summary>
    /// <param name="id">Discount ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Discount deleted.</response>
    /// <response code="404">Discount not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteDiscount(Guid id, CancellationToken cancellationToken)
    {
        await _discountCommandService.DeleteDiscountAsync(id, cancellationToken);
        return NoContent();
    }
}
