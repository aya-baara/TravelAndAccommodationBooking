using AutoMapper;
using BookingPlatform.Application.Dtos.Invoices;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Core.Constants;
using BookingPlatform.WebAPI.Dtos.Invoices;
using BookingPlatform.WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingPlatform.WebAPI.Controllers;

[ApiController]
[Route("api/bookings/{bookingId:guid}/invoices")]

public class InvoiceController : ControllerBase
{
    private readonly IInvoiceCommandService _invoiceCommandService;
    private readonly IInvoiceQueryService _invoiceQueryService;
    private readonly IMapper _mapper;

    public InvoiceController(IInvoiceCommandService invoiceCommandService,
                             IInvoiceQueryService invoiceQueryService,
                             IMapper mapper)
    {
        _invoiceCommandService = invoiceCommandService;
        _invoiceQueryService = invoiceQueryService;
        _mapper = mapper;
    }

    /// <summary>
    /// Create a new invoice.
    /// </summary>
    /// <param name="bookingId">The ID of the booking associated with the invoice.</param>
    /// <param name="request"></param>
    /// <param name="dto">The invoice details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created invoice.</returns>
    /// <response code="201">Invoice created successfully.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden (Not Admin).</response>
    [HttpPost]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(typeof(InvoiceResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateInvoice([FromRoute] Guid bookingId, [FromBody] CreateInvoiceRequestDto request, CancellationToken cancellationToken)
    {
        var dto = _mapper.Map<CreateInvoiceDto>(request);
        dto.BookingId = bookingId;
        var result = await _invoiceCommandService.CreateInvoiceAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetInvoiceById), new { bookingId, invoiceId = result.Id }, result);
    }

    /// <summary>
    /// Get invoice by ID.
    /// </summary>
    /// <param name="bookingId">The ID of the booking associated with the invoice.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The invoice details.</returns>
    /// <response code="200">Invoice found.</response>
    /// <response code="404">Invoice not found.</response>
    /// <response code="401">Unauthorized.</response>

    [HttpGet]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(typeof(InvoiceResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]

    public async Task<IActionResult> GetInvoiceById([FromRoute] Guid bookingId, CancellationToken cancellationToken)
    {
        var result = await _invoiceQueryService.GetInvoiceByBookingId(bookingId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Update an existing invoice.
    /// </summary>
    /// <param name="bookingId">The ID of the booking associated with the invoice.</param>
    /// <param name="request"></param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">Invoice updated successfully.</response>
    /// <response code="404">Invoice not found.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden (Not Admin).</response>
    [HttpPut]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateInvoice([FromRoute]Guid bookingId, [FromBody] UpdateInvoiceRequestDto request, CancellationToken cancellationToken)
    {
        var dto = _mapper.Map<UpdateInvoiceDto>(request);
        dto.BookingId = bookingId;
        await _invoiceCommandService.UpdateInvoiceAsync(dto, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Delete an invoice by ID.
    /// </summary>
    /// <param name="bookingId">Invoice ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content if deleted.</returns>
    /// <response code="204">Invoice deleted successfully.</response>
    /// <response code="404">Invoice not found.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden (Not Admin).</response>
    [HttpDelete]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteInvoice([FromRoute] Guid bookingId, CancellationToken cancellationToken)
    {
        await _invoiceCommandService.DeleteInvoiceAsync(bookingId, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Generate and download a PDF invoice for the authenticated user.
    /// </summary>
    /// <param name="bookingId">The ID of the booking associated with the invoice.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>PDF file of the invoice.</returns>
    /// <response code="200">Invoice generated successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to access this invoice.</response>
    /// <response code="404">Invoice not found.</response>
    [HttpGet("print")]
    [Authorize]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PrintInvoice([FromRoute] Guid bookingId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId(); ;
        var pdfBytes = await _invoiceQueryService.PrintInvoice(bookingId, userId, cancellationToken);
        return File(pdfBytes, "application/pdf", $"invoice-{bookingId}.pdf");
    }
}
