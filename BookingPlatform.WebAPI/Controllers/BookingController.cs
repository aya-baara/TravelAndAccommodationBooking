using BookingPlatform.Application.Dtos.Bookings;
using BookingPlatform.Application.Dtos.Hotels;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingPlatform.WebAPI.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingCommandService _bookingCommandService;
    private readonly IBookingQueryService _bookingQueryService;

    public BookingController(IBookingCommandService bookingCommandService
        , IBookingQueryService bookingQueryService)
    {
        _bookingCommandService = bookingCommandService;
        _bookingQueryService = bookingQueryService;
    }

    /// <summary>
    /// Create a new booking for the authenticated user.
    /// </summary>
    /// <param name="request">Booking creation details including room IDs, check-in/out dates, and remarks.</param>
    /// <returns>The created booking details.</returns>
    /// <response code="201">Booking created successfully.</response>
    /// <response code="400">Invalid input data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">One or more rooms not found.</response>
    [HttpPost]
    [ProducesResponseType(typeof(BookingResponseDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var bookingDto = new CreateBookingDto
        {
            RoomIds = request.RoomIds,
            Remarks = request.Remarks,
            CheckIn = request.CheckIn,
            CheckOut = request.CheckOut
        };

        var createdBooking = await _bookingCommandService.CreateBookingAsync(bookingDto, userId, cancellationToken);
        return CreatedAtAction(nameof(GetBookingById), new { id = createdBooking.Id }, createdBooking);
    }

    /// <summary>
    /// Get booking details by booking ID for the authenticated user.
    /// </summary>
    /// <param name="id">Booking ID.</param>
    /// <returns>Booking details.</returns>
    /// <response code="200">Booking retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Booking not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BookingResponseDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetBookingById(Guid id, CancellationToken cancellationToken)
    {
        var booking = await _bookingQueryService.GetBookingByIdAsync(id, cancellationToken);
        return Ok(booking);
    }

    /// <summary>
    /// Delete a booking by ID for the authenticated user.
    /// </summary>
    /// <param name="id">Booking ID to delete.</param>
    /// <returns>No content.</returns>
    /// <response code="204">Booking deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to delete this booking.</response>
    /// <response code="404">Booking not found.</response>
    /// <response code="409">Deletion is not allowed due to business rules.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> DeleteBooking(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _bookingCommandService.DeleteBookingAsync(id, userId, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Update booking remarks by booking ID for the authenticated user.
    /// </summary>
    /// <param name="dto">UpdateBookingDto with booking ID, user ID, and remarks.</param>
    /// <returns>No content.</returns>
    /// <response code="204">Booking updated successfully.</response>
    /// <response code="400">Invalid input data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to update this booking.</response>
    /// <response code="404">Booking not found.</response>
    [HttpPut]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateBooking([FromBody] UpdateBookingDto dto, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        await _bookingCommandService.UpdateBookingAsync(dto, userId, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Get recently booked hotels for the authenticated user.
    /// </summary>
    /// <param name="count">Number of recent hotels to retrieve.</param>
    /// <returns>List of recently booked hotels.</returns>
    /// <response code="200">Successfully retrieved recent hotels.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("recent-hotels")]
    [ProducesResponseType(typeof(List<RecentHotelDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetRecentHotels([FromQuery] int count = 5, CancellationToken cancellationToken = default)
    {
        var userId = User.GetUserId();
        var hotels = await _bookingQueryService.GetRecentHotelsForUserAsync(userId, count, cancellationToken);
        return Ok(hotels);
    }
}
