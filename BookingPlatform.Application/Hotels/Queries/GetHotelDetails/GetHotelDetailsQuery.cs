using BookingPlatform.Application.Hotels.Dtos;
using MediatR;

namespace BookingPlatform.Application.Hotels.Queries.GetHotelDetails;

public class GetHotelDetailsQuery: IRequest<HotelDetailsDto>
{
    public Guid HotelId { get; set; }
}

