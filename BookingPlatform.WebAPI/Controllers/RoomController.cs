using BookingPlatform.Application.Dtos.Rooms;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace BookingPlatform.WebAPI.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomController : ControllerBase
{
    private readonly IRoomCommandService _roomCommandService;
    private readonly IRoomQueryService _roomQueryService;

    public RoomController(IRoomCommandService roomCommandService, IRoomQueryService roomQueryService)
    {
        _roomCommandService = roomCommandService;
        _roomQueryService = roomQueryService;
    }

    /// <summary>
    /// Create a new room.
    /// </summary>
    /// <param name="dto">The room details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created room.</returns>
    /// <response code="201">Room created successfully.</response>
    /// <response code="404">Hotel not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpPost]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(typeof(RoomResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<RoomResponseDto>> CreateRoom([FromBody] CreateRoomDto dto, CancellationToken cancellationToken)
    {
        var result = await _roomCommandService.CreateRoomAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetRoomById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing room.
    /// </summary>
    /// <param name="dto">Updated room details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Room updated successfully.</response>
    /// <response code="404">Room not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpPut]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateRoom([FromBody] UpdateRoomDto dto, CancellationToken cancellationToken)
    {
        await _roomCommandService.UpdateRoomAsync(dto, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Delete a room by ID.
    /// </summary>
    /// <param name="id">Room ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Room deleted successfully.</response>
    /// <response code="404">Room not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteRoom(Guid id, CancellationToken cancellationToken)
    {
        await _roomCommandService.DeleteRoomByIdAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Get a room by ID.
    /// </summary>
    /// <param name="id">Room ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested room.</returns>
    /// <response code="200">Room retrieved successfully.</response>
    /// <response code="404">Room not found.</response>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RoomResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomResponseDto>> GetRoomById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _roomQueryService.GetRoomByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get available rooms in a hotel.
    /// </summary>
    /// <param name="hotelId">Hotel ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of available rooms.</returns>
    /// <response code="200">Available rooms retrieved successfully.</response>
    [HttpGet("hotel/{hotelId:guid}/available")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<RoomResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<RoomResponseDto>>> GetAvailableRoomsByHotel(Guid hotelId, CancellationToken cancellationToken)
    {
        var result = await _roomQueryService.GetAvailableRoomsByHotelAsync(hotelId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Search for rooms with filtering and pagination (admin only).
    /// </summary>
    /// <param name="request">Search and pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of rooms.</returns>
    /// <response code="200">Rooms retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpGet("admin")]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(typeof(PaginatedResult<RoomManagementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaginatedResult<RoomManagementDto>>> SearchRoomsAdmin([FromQuery] SieveModel request, CancellationToken cancellationToken)
    {
        var result = await _roomQueryService.SearchRoomsAsync(request, cancellationToken);
        return Ok(result);
    }
}
