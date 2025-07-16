using BookingPlatform.Application.Dtos.Cities;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace BookingPlatform.WebAPI.Controllers;

[ApiController]
[Route("api/cities")]
[Authorize(Roles = RoleNames.Admin)]
public class CityController : ControllerBase
{
    private readonly ICityCommandService _cityCommandService;
    private readonly ICityQueryService _cityQueryService;

    public CityController(ICityCommandService cityCommandService, ICityQueryService cityQueryService)
    {
        _cityCommandService = cityCommandService;
        _cityQueryService = cityQueryService;
    }

    /// <summary>
    /// Create a new city.
    /// </summary>
    /// <param name="dto">The city details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created city.</returns>
    /// <response code="201">City created successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpPost]
    [ProducesResponseType(typeof(CityResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateCity([FromBody] CreateCityDto dto, CancellationToken cancellationToken)
    {
        var city = await _cityCommandService.CreateCityAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetCityById), new { id = city.Id}, city);
    }

    /// <summary>
    /// Update an existing city.
    /// </summary>
    /// <param name="dto">Updated city data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">City updated successfully.</response>
    /// <response code="404">If the city is not found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateCity([FromBody] UpdateCityDto dto, CancellationToken cancellationToken)
    {
        await _cityCommandService.UpdateCityAsync(dto, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Delete a city by ID.
    /// </summary>
    /// <param name="id">City ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">City deleted successfully.</response>
    /// <response code="404">If the city is not found.</response> 
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteCity(Guid id, CancellationToken cancellationToken)
    {
        await _cityCommandService.DeleteCityAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Get a city by ID.
    /// </summary>
    /// <param name="id">City ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested city.</returns>
    /// <response code="200">Returns the city.</response>
    /// <response code="404">If the city is not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CityResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<IActionResult> GetCityById(Guid id, CancellationToken cancellationToken)
    {
        var city = await _cityQueryService.GetCityByIdAsync(id, cancellationToken);
        return Ok(city);
    }

    /// <summary>
    /// Get top N most visited cities.
    /// </summary>
    /// <param name="num">Number of top cities to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of top visited cities.</returns>
    /// <response code="200">Returns top visited cities.</response>
    [HttpGet("top/{num}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<CityResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopVisitedCities(int num, CancellationToken cancellationToken)
    {
        var cities = await _cityQueryService.GetTopVisitedCities(num, cancellationToken);
        return Ok(cities);
    }

    /// <summary>
    /// Search cities with filters, sorting, and pagination (for admin).
    /// </summary>
    /// <param name="request">Filtering, sorting, and pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of cities.</returns>
    /// <response code="200">Returns search results.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized (not an admin).</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PaginatedResult<CityManagementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SearchCities([FromQuery] SieveModel request, CancellationToken cancellationToken)
    {
        var result = await _cityQueryService.SearchCitiesAsync(request, cancellationToken);
        return Ok(result);
    }
}