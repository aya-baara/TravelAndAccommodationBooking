using AutoMapper;
using BookingPlatform.Application.Dtos.Users;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Application.Interfaces.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BookingPlatform.WebAPI.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserCommandService _userCommandService;
    private readonly IUserQueryService _userQueryService;
    private readonly IRoleQueryService _roleQueryService;
    private readonly IMapper _mapper;

    public UserController(IUserCommandService userCommandService
        , IUserQueryService userQueryService
        ,IRoleQueryService roleQueryService
        ,IMapper mapper)
    {
        _userCommandService = userCommandService;
        _userQueryService = userQueryService;
        _roleQueryService = roleQueryService;
        _mapper = mapper;
    }

    /// <summary>
    /// Register a new user.
    /// </summary>
    /// <param name="dto">The user data for sign up.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">User signed up successfully.</response>
    /// <response code="404">If the provided RoleId does not exist.</response>
    /// <response code="400">If the email already exists.</response>
    /// <returns>No content on success.</returns>
    [HttpPost("signup")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SignUp([FromBody] SignUpDto dto, CancellationToken cancellationToken)
    {
        var user= _mapper.Map<CreateUserDto>(dto);
        user.RoleId = (await _roleQueryService.GetRoleByTypeAsync(Core.Enums.RoleType.User, cancellationToken)).Id;
        await _userCommandService.SignUpAsync(user, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Authenticate a user and return a JWT token.
    /// </summary>
    /// <param name="dto">The login credentials.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A JWT access token.</returns>
    /// <response code="200">Returns the JWT token.</response>
    /// <response code="401">If the email or password is incorrect.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LogIn([FromBody] LogInDto dto, CancellationToken cancellationToken)
    {
        var result = await _userQueryService.LogInAsync(dto, cancellationToken);
        return Ok(result);
    }
}
