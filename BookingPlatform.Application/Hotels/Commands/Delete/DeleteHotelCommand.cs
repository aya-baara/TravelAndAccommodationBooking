using MediatR;

namespace BookingPlatform.Application.Hotels.Commands.Delete;

public class DeleteHotelCommand : IRequest
{
    public Guid HotelId { get; init; }
}

