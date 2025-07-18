using AutoMapper;
using BookingPlatform.Application.Dtos.Invoices;
using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Mapping;

public class InvoiceProfile : Profile
{
    public InvoiceProfile()
    {
        CreateMap<CreateInvoiceDto, Invoice>()
            .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.BookingId));
        CreateMap<UpdateInvoiceDto, Invoice>();
        CreateMap<Invoice, InvoiceResponseDto>();
    }
}

