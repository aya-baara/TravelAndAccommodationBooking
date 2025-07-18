using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using BookingPlatform.Application.Dtos.Invoices;
using BookingPlatform.Application.Services.Commands;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces;
using BookingPlatform.Core.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookingPlatform.Tests.Commands;

public class InvoiceCommandServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IInvoiceRopsitory> _invoiceRepoMock;
    private readonly Mock<IBookingRepository> _bookingRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<InvoiceCommandService>> _loggerMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly InvoiceCommandService _sut;

    public InvoiceCommandServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Customize<DateOnly>(c =>
         c.FromFactory(() => DateOnly.FromDateTime(DateTime.Today.AddYears(-_fixture.Create<int>() % 40 - 20)))
        );
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _invoiceRepoMock = new Mock<IInvoiceRopsitory>();
        _bookingRepoMock = new Mock<IBookingRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<InvoiceCommandService>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _sut = new InvoiceCommandService(
            _invoiceRepoMock.Object,
            _bookingRepoMock.Object,
            _mapperMock.Object,
            _loggerMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateInvoiceAsync_ShouldReturnDto_WhenBookingExists()
    {
        var dto = _fixture.Create<CreateInvoiceDto>();
        var booking = _fixture.Build<Booking>()
            .With(b => b.Id, dto.BookingId)
            .With(b => b.TotalPriceBeforeDiscount, 100m)
            .Create();
        var invoice = _fixture.Build<Invoice>().With(i => i.BookingId, dto.BookingId).Create();
        var createdInvoice = _fixture.Build<Invoice>().With(i => i.BookingId, dto.BookingId).Create();
        var responseDto = _fixture.Create<InvoiceResponseDto>();

        _bookingRepoMock.Setup(r => r.GetBookingById(dto.BookingId, default)).ReturnsAsync(booking);
        _mapperMock.Setup(m => m.Map<Invoice>(dto)).Returns(invoice);
        _invoiceRepoMock.Setup(r => r.CreateInvoiceAsync(invoice, default)).ReturnsAsync(createdInvoice);
        _invoiceRepoMock.Setup(r => r.GetInvoiceByBookingIdAsync(createdInvoice.BookingId, default)).ReturnsAsync(createdInvoice);
        _mapperMock.Setup(m => m.Map<InvoiceResponseDto>(createdInvoice)).Returns(responseDto);

        var result = await _sut.CreateInvoiceAsync(dto, default);

        result.Should().BeEquivalentTo(responseDto);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateInvoiceAsync_ShouldThrow_WhenBookingNotFound()
    {
        var dto = _fixture.Create<CreateInvoiceDto>();
        _bookingRepoMock.Setup(r => r.GetBookingById(dto.BookingId, default)).ReturnsAsync((Booking)null!);

        Func<Task> act = async () => await _sut.CreateInvoiceAsync(dto, default);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("The Requested Booking Not found");
    }

    [Fact]
    public async Task DeleteInvoiceAsync_ShouldSucceed_WhenInvoiceExists()
    {
        var bookingId = Guid.NewGuid();
        var invoice = _fixture.Build<Invoice>().With(i => i.BookingId, bookingId).Create();

        _invoiceRepoMock.Setup(r => r.GetInvoiceByBookingIdAsync(bookingId, default)).ReturnsAsync(invoice);

        await _sut.DeleteInvoiceAsync(bookingId, default);

        _invoiceRepoMock.Verify(r => r.DeleteInvoiceAsync(bookingId, default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteInvoiceAsync_ShouldThrow_WhenInvoiceNotFound()
    {
        var bookingId = Guid.NewGuid();

        _invoiceRepoMock.Setup(r => r.GetInvoiceByBookingIdAsync(bookingId, default)).ReturnsAsync((Invoice)null!);

        Func<Task> act = async () => await _sut.DeleteInvoiceAsync(bookingId, default);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("The Requested Invoice Not found");
    }

    [Fact]
    public async Task UpdateInvoiceAsync_ShouldSucceed_WhenInvoiceExists()
    {
        var dto = _fixture.Create<UpdateInvoiceDto>();
        var existingInvoice = _fixture.Build<Invoice>().With(i => i.BookingId, dto.BookingId).Create();
        var updatedInvoice = _fixture.Build<Invoice>().With(i => i.BookingId, dto.BookingId).Create();

        _invoiceRepoMock.Setup(r => r.GetInvoiceByBookingIdAsync(dto.BookingId, default)).ReturnsAsync(existingInvoice);
        _mapperMock.Setup(m => m.Map<Invoice>(dto)).Returns(updatedInvoice);

        await _sut.UpdateInvoiceAsync(dto, default);

        _invoiceRepoMock.Verify(r => r.UpdateInvoiceAsync(It.Is<Invoice>(i => i.Id == existingInvoice.Id), default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateInvoiceAsync_ShouldThrow_WhenInvoiceNotFound()
    {
        var dto = _fixture.Create<UpdateInvoiceDto>();

        _invoiceRepoMock.Setup(r => r.GetInvoiceByBookingIdAsync(dto.BookingId, default)).ReturnsAsync((Invoice)null!);

        Func<Task> act = async () => await _sut.UpdateInvoiceAsync(dto, default);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("The Requested Invoice Not found");
    }
}
