using BookingPlatform.Application.Dtos.Invoices;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Core.Constants;
using BookingPlatform.WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingPlatform.WebAPI.Controllers;

[ApiController]
[Route("api/invoices")]
[Authorize(Roles = RoleNames.Admin)]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceCommandService _invoiceCommandService;
    private readonly IInvoiceQueryService _invoiceQueryService;

    public InvoiceController(IInvoiceCommandService invoiceCommandService,
                             IInvoiceQueryService invoiceQueryService)
    {
        _invoiceCommandService = invoiceCommandService;
        _invoiceQueryService = invoiceQueryService;
    }

    /// <summary>
    /// Create a new invoice.
    /// </summary>
    /// <param name="dto">The invoice details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created invoice.</returns>
    /// <response code="201">Invoice created successfully.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden (Not Admin).</response>
    [HttpPost]
    [ProducesResponseType(typeof(InvoiceResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto dto, CancellationToken cancellationToken)
    {
        var result = await _invoiceCommandService.CreateInvoiceAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetInvoiceById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Get invoice by ID.
    /// </summary>
    /// <param name="id">Invoice ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The invoice details.</returns>
    /// <response code="200">Invoice found.</response>
    /// <response code="404">Invoice not found.</response>
    /// <response code="401">Unauthorized.</response>

    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(InvoiceResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]

    public async Task<IActionResult> GetInvoiceById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId(); ;
        var result = await _invoiceQueryService.GetInvoiceById(id, userId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Update an existing invoice.
    /// </summary>
    /// <param name="dto">Updated invoice data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Invoice updated successfully.</response>
    /// <response code="404">Invoice not found.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden (Not Admin).</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateInvoice([FromBody] UpdateInvoiceDto dto, CancellationToken cancellationToken)
    {
        await _invoiceCommandService.UpdateInvoiceAsync(dto, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Delete an invoice by ID.
    /// </summary>
    /// <param name="id">Invoice ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content if deleted.</returns>
    /// <response code="204">Invoice deleted successfully.</response>
    /// <response code="404">Invoice not found.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden (Not Admin).</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteInvoice([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _invoiceCommandService.DeleteInvoiceAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Generate and download a PDF invoice for the authenticated user.
    /// </summary>
    /// <param name="id">The ID of the invoice to print.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>PDF file of the invoice.</returns>
    /// <response code="200">Invoice generated successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to access this invoice.</response>
    /// <response code="404">Invoice not found.</response>
    [HttpGet("{id:guid}/print")]
    [Authorize]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PrintInvoice([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId(); ;
        var pdfBytes = await _invoiceQueryService.PrintInvoice(id, userId, cancellationToken);
        return File(pdfBytes, "application/pdf", $"invoice-{id}.pdf");
    }
}
