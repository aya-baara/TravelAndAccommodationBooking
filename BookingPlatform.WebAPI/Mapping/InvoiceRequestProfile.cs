using AutoMapper;
using BookingPlatform.Application.Dtos.Invoices;
using BookingPlatform.WebAPI.Dtos.Invoices;

namespace BookingPlatform.WebAPI.Mapping;

public class InvoiceRequestProfile : Profile
{
    public InvoiceRequestProfile()
    {
        CreateMap<CreateInvoiceRequestDto, CreateInvoiceDto>()
            .ForMember(dest => dest.BookingId, opt => opt.Ignore());

        CreateMap<UpdateInvoiceRequestDto, UpdateInvoiceDto>()
            .ForMember(dest => dest.BookingId, opt => opt.Ignore());

    }
}

