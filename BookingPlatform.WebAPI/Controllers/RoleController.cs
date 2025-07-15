using BookingPlatform.Application.Dtos.Roles;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Core.Constants;
using BookingPlatform.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingPlatform.WebAPI.Controllers;

[ApiController]
[Route("api/roles")]
[Authorize(Roles = RoleNames.Admin)]

public class RoleController : ControllerBase
{
    private readonly IRoleCommandService _roleCommandService;
    private readonly IRoleQueryService _roleQueryService;

    public RoleController(IRoleCommandService roleCommandService, IRoleQueryService roleQueryService)
    {
        _roleCommandService = roleCommandService;
        _roleQueryService = roleQueryService;
    }

    /// <summary>
    /// Create a new role.
    /// </summary>
    /// <param name="dto">The role details to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created role.</returns>
    /// <response code="201">Returns the newly created role</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpPost]
    [ProducesResponseType(typeof(RoleResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto, CancellationToken cancellationToken)
    {
        var created = await _roleCommandService.CreateRoleAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetRoleById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Get role details by ID.
    /// </summary>
    /// <param name="id">The ID of the role.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The role data.</returns>
    /// <response code="200">Returns the requested role</response>
    /// <response code="404">If the role is not found</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RoleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetRoleById(Guid id, CancellationToken cancellationToken)
    {
        var role = await _roleQueryService.GetRoleByIdAsync(id, cancellationToken);
        return Ok(role);
    }

    /// <summary>
    /// Get role details by role type.
    /// </summary>
    /// <param name="roleType">The enum name of the role (e.g., "Admin", "User").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The role data.</returns>
    /// <response code="200">Returns the requested role</response>
    /// <response code="404">If the role is not found</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpGet("by-type/{roleType}")]
    [ProducesResponseType(typeof(RoleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetRoleByType(RoleType roleType, CancellationToken cancellationToken)
    {
        var role = await _roleQueryService.GetRoleByTypeAsync(roleType, cancellationToken);
        return Ok(role);
    }

    /// <summary>
    /// Delete a role by ID.
    /// </summary>
    /// <param name="id">The ID of the role to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Role deleted successfully</response>
    /// <response code="404">If the role is not found</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteRole(Guid id, CancellationToken cancellationToken)
    {
        await _roleCommandService.DeleteRoleAsync(id, cancellationToken);
        return NoContent();
    }
}
