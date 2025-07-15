using BookingPlatform.Application.Dtos.Owners;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingPlatform.WebAPI.Controllers;

[ApiController]
[Route("api/owners")]
[Authorize(Roles = RoleNames.Admin)]

public class OwnerController : ControllerBase
{
    private readonly IOwnerCommandService _ownerCommandService;
    private readonly IOwnerQueryService _ownerQueryService;

    public OwnerController(IOwnerCommandService ownerCommandService
        , IOwnerQueryService ownerQueryService)
    {
        _ownerCommandService = ownerCommandService;
        _ownerQueryService = ownerQueryService;
    }
    /// <summary>
    /// Create a new owner.
    /// </summary>
    /// <param name="dto">The owner details to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created owner.</returns>
    /// <response code="201">Returns the newly created owner</response>
    /// <response code="400">If the input is invalid</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpPost]
    [ProducesResponseType(typeof(OwnerResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateOwner([FromBody] CreateOwnerDto dto, CancellationToken cancellationToken)
    {
        var result = await _ownerCommandService.CreateOwnerAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetOwnerById), new { id = result.Id }, result);
    }
    /// <summary>
    /// Update an existing owner.
    /// </summary>
    /// <param name="dto">The updated owner information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Owner successfully updated</response>
    /// <response code="404">If the owner is not found</response>
    /// <response code="400">If the input is invalid</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateOwner([FromBody] UpdateOwnerDto dto, CancellationToken cancellationToken)
    {
        await _ownerCommandService.UpdateOwnerAsync(dto, cancellationToken);
        return NoContent(); 
    }
    /// <summary>
    /// Delete an owner by ID.
    /// </summary>
    /// <param name="id">The ID of the owner to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Owner successfully deleted</response>
    /// <response code="404">If the owner is not found</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteOwner(Guid id, CancellationToken cancellationToken)
    {
        await _ownerCommandService.DeleteOwnerAsync(id, cancellationToken);
        return NoContent(); 
    }

    /// <summary>
    /// Get a specific owner by ID.
    /// </summary>
    /// <param name="id">The ID of the owner to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An OwnerResponseDto object.</returns>
    /// <response code="200">Returns the requested owner</response>
    /// <response code="404">If the owner is not found</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OwnerResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetOwnerById(Guid id, CancellationToken cancellationToken)
    {
        var owner = await _ownerQueryService.GetOwnerByIdAsync(id, cancellationToken);
        return Ok(owner);
    }
}