using AutoMapper;
using BookingPlatform.Application.Dtos.Invoices;
using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Mapping;

public class InvoiceProfile : Profile
{
    public InvoiceProfile()
    {
        CreateMap<CreateInvoiceDto, Invoice>();
        CreateMap<UpdateInvoiceDto, Invoice>();
        CreateMap<Invoice, InvoiceResponseDto>();
    }
}

