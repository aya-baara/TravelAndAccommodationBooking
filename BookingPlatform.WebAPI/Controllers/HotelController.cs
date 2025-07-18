using BookingPlatform.Application.Dtos.Hotels;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace BookingPlatform.WebAPI.Controllers;

[ApiController]
[Route("api/hotels")]
public class HotelController : ControllerBase
{
    private readonly IHotelCommandService _hotelCommandService;
    private readonly IHotelQueryService _hotelQueryService;

    public HotelController(IHotelCommandService hotelCommandService, IHotelQueryService hotelQueryService)
    {
        _hotelCommandService = hotelCommandService;
        _hotelQueryService = hotelQueryService;
    }

    /// <summary>
    /// Create a new hotel.
    /// </summary>
    /// <param name="dto">Hotel creation details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created hotel details.</returns>
    /// <response code="201">Returns the newly created hotel.</response>
    /// <response code="404">City or Owner not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpPost]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(typeof(HotelResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<HotelResponseDto>> CreateHotel([FromBody] CreateHotelDto dto, CancellationToken cancellationToken)
    {
        var created = await _hotelCommandService.CreateHotelAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetHotelById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Get hotel details by ID.
    /// </summary>
    /// <param name="id">Hotel ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Hotel details including images and reviews.</returns>
    /// <response code="200">Returns the hotel details.</response>
    /// <response code="404">Hotel not found.</response>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(HotelDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HotelDetailsDto>> GetHotelById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _hotelQueryService.GetHotelDetailsByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Search hotels with filters and pagination.
    /// </summary>
    /// <param name="request">Search filters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Paginated list of hotels.</returns>
    /// <response code="200">Returns a list of hotels.</response>
    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaginatedResult<HotelSearchDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<HotelSearchDto>>> SearchHotels([FromQuery] HotelSearchRequest request, CancellationToken ct)
    {
        var result = await _hotelQueryService.SearchHotelsAsync(request, ct);
        return Ok(result);
    }

    /// <summary>
    /// Search hotels for admin with filters.
    /// </summary>
    /// <param name="request">Admin search filters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Paginated admin hotel view.</returns>
    /// <response code="200">Returns admin hotel data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpGet("admin")]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(typeof(PaginatedResult<HotelManagementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaginatedResult<HotelManagementDto>>> SearchHotelsAdmin([FromQuery] SieveModel request, CancellationToken ct)
    {
        var result = await _hotelQueryService.SearchHotelsAdminAsync(request, ct);
        return Ok(result);
    }

    /// <summary>
    /// Get featured hotel deals.
    /// </summary>
    /// <param name="count">Number of deals to fetch.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of featured deals.</returns>
    /// <response code="200">Returns featured deals.</response>
    [HttpGet("featured")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<FeaturedHotelDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FeaturedHotelDto>>> GetFeaturedDeals([FromQuery] int count, CancellationToken ct)
    {
        var result = await _hotelQueryService.GetFeaturedDealsAsync(count, ct);
        return Ok(result);
    }

    /// <summary>
    /// Update a hotel.
    /// </summary>
    /// <param name="dto">Hotel update data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Hotel updated successfully.</response>
    /// <response code="404">Hotel not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpPut]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateHotel([FromBody] UpdateHotelDto dto, CancellationToken cancellationToken)
    {
        await _hotelCommandService.UpdateHotelAsync(dto, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Delete a hotel by ID.
    /// </summary>
    /// <param name="id">Hotel ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Hotel deleted.</response>
    /// <response code="404">Hotel not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteHotel(Guid id, CancellationToken cancellationToken)
    {
        await _hotelCommandService.DeleteHotelAsync(id, cancellationToken);
        return NoContent();
    }
}
