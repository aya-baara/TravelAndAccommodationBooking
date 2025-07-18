using AutoMapper;
using BookingPlatform.Application.Dtos.Users;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingPlatform.WebAPI.Controllers;

[ApiController]
[Route("api/admins")]
[Authorize(Roles = RoleNames.Admin)]
public class AdminController : ControllerBase
{
    private readonly IUserCommandService _userCommandService;
    private readonly IRoleQueryService _roleQueryService;
    private readonly IMapper _mapper;

    public AdminController(IUserCommandService userCommandService
        , IRoleQueryService roleQueryService
        , IMapper mapper)
    {
        _userCommandService = userCommandService;
        _roleQueryService = roleQueryService;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new Admin user.
    /// </summary>
    /// <param name="dto">The sign-up details including email and password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Admin user created successfully.</response>
    /// <response code="400">Invalid input data or validation failed.</response>
    /// <response code="401">User is not authenticated or token invalid.</response>
    /// <response code="403">User is not authorized to perform this action.</response>
    /// <response code="404">Specified role (Admin) was not found.</response>
    /// <response code="409">Email already exists.</response>
    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> CreteAdmin([FromBody] SignUpDto dto, CancellationToken cancellationToken)
    {
        var user = _mapper.Map<CreateUserDto>(dto);
        user.RoleId = (await _roleQueryService.GetRoleByTypeAsync(Core.Enums.RoleType.Admin, cancellationToken)).Id;
        await _userCommandService.SignUpAsync(user, cancellationToken);
        return NoContent();
    }
}

