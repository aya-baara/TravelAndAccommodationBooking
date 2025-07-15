using BookingPlatform.Application.Dtos.Images;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingPlatform.WebAPI.Controllers;

[ApiController]
[Route("api/images")]
[Authorize(Roles = RoleNames.Admin)]
public class ImageController : ControllerBase
{
    private readonly IImageCommandService _imageCommandService;
    private readonly IImageQueryService _imageQueryService;

    public ImageController(IImageCommandService imageCommandService,
                           IImageQueryService imageQueryService)
    {
        _imageCommandService = imageCommandService;
        _imageQueryService = imageQueryService;
    }

    /// <summary>
    /// Create a new image for a hotel or room.
    /// </summary>
    /// <param name="dto">The image details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created image.</returns>
    /// <response code="201">Image created successfully.</response>
    /// <response code="404">Hotel/room not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpPost]
    [ProducesResponseType(typeof(ImageResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateImage([FromBody] CreateImageDto dto, CancellationToken cancellationToken)
    {
        var result = await _imageCommandService.CreateImageAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetImageById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Get an image by its ID.
    /// </summary>
    /// <param name="id">The image ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested image.</returns>
    /// <response code="200">Image retrieved successfully.</response>
    /// <response code="404">Image not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ImageResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetImageById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _imageQueryService.GetImageByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Delete an image by ID.
    /// </summary>
    /// <param name="id">The image ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Image deleted successfully.</response>
    /// <response code="404">Image not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteImage(Guid id, CancellationToken cancellationToken)
    {
        await _imageCommandService.DeleteImageAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Update an existing image.
    /// </summary>
    /// <param name="dto">The image update details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Image updated successfully.</response>
    /// <response code="404">Image not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized.</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateImage([FromBody] UpdateImageDto dto, CancellationToken cancellationToken)
    {
        await _imageCommandService.UpdateImageAsync(dto, cancellationToken);
        return NoContent();
    }
}
