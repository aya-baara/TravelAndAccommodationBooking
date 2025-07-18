using AutoMapper;
using BookingPlatform.Application.Dtos.Invoices;
using BookingPlatform.Application.Services.Queries;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookingPlatform.Tests.Queries;

public class InvoiceQueryServiceTests : IClassFixture<InvoiceQueryServiceTests.Fixture>
{
    private readonly Fixture _fixture;

    public InvoiceQueryServiceTests(Fixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetInvoiceByBookingId_ShouldReturnInvoice_WhenExists()
    {
        // Arrange
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            Booking = new Booking { Id = Guid.NewGuid() }
        };

        _fixture.InvoiceRepoMock
            .Setup(r => r.GetInvoiceByBookingIdAsync(invoice.Booking.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        _fixture.MapperMock
            .Setup(m => m.Map<InvoiceResponseDto>(invoice))
            .Returns(new InvoiceResponseDto());

        var service = _fixture.CreateService();

        // Act
        var result = await service.GetInvoiceByBookingId(invoice.Booking.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _fixture.InvoiceRepoMock.Verify(r => r.GetInvoiceByBookingIdAsync(invoice.Booking.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetInvoiceByBookingId_ShouldThrowNotFound_WhenNotExists()
    {
        // Arrange
        _fixture.InvoiceRepoMock
            .Setup(r => r.GetInvoiceByBookingIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice)null);

        var service = _fixture.CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.GetInvoiceByBookingId(Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task PrintInvoice_ShouldReturnPdf_WhenAuthorized()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            Booking = new Booking { Id = bookingId, UserId = userId }
        };

        _fixture.InvoiceRepoMock
            .Setup(r => r.GetInvoiceByBookingIdAsync(bookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        _fixture.HtmlBuilderMock
            .Setup(h => h.BuildInvoiceDetailsHtml(invoice))
            .Returns("<html><body>Invoice</body></html>");

        _fixture.PdfServiceMock
            .Setup(p => p.GeneratePdfFromHtml(It.IsAny<string>()))
            .Returns(new byte[] { 0x25, 0x50 });

        var service = _fixture.CreateService();

        // Act
        var result = await service.PrintInvoice(bookingId, userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task PrintInvoice_ShouldThrowNotFound_WhenInvoiceMissing()
    {
        // Arrange
        _fixture.InvoiceRepoMock
            .Setup(r => r.GetInvoiceByBookingIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Invoice)null);

        var service = _fixture.CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.PrintInvoice(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task PrintInvoice_ShouldThrowForbidden_WhenUnauthorizedUser()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            Booking = new Booking { Id = bookingId, UserId = Guid.NewGuid() }
        };

        _fixture.InvoiceRepoMock
            .Setup(r => r.GetInvoiceByBookingIdAsync(bookingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var service = _fixture.CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenAccessException>(() =>
            service.PrintInvoice(bookingId, Guid.NewGuid(), CancellationToken.None));
    }

    public class Fixture
    {
        public Mock<IInvoiceRopsitory> InvoiceRepoMock { get; } = new();
        public Mock<IMapper> MapperMock { get; } = new();
        public Mock<ILogger<InvoiceQueryService>> LoggerMock { get; } = new();
        public Mock<IPdfService> PdfServiceMock { get; } = new();
        public Mock<IInvoiceHtmlBuilder> HtmlBuilderMock { get; } = new();

        public InvoiceQueryService CreateService()
        {
            return new InvoiceQueryService(
                InvoiceRepoMock.Object,
                MapperMock.Object,
                LoggerMock.Object,
                PdfServiceMock.Object,
                HtmlBuilderMock.Object
            );
        }
    }
}
