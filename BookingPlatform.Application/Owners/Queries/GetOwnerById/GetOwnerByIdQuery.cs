using BookingPlatform.Application.Owners.Dtos;
using MediatR;

namespace BookingPlatform.Application.Owners.Queries.GetOwnerById;

public class GetOwnerByIdQuery :IRequest<OwnerResponseDto>
{
    public Guid Id { get; set; }
}

