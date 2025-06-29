using AutoMapper;
using BookingPlatform.Application.Hotels.Commands.Update;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using MediatR;

namespace BookingPlatform.Application.Hotels.Commands.Delete;

public class DeleteHotelCommandHandler : IRequestHandler<DeleteHotelCommand>
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IMapper _mapper;
    public DeleteHotelCommandHandler(IHotelRepository hotelRepository, IMapper mapper)
    {
        _hotelRepository = hotelRepository;
        _mapper = mapper;
    }
    public async Task Handle(DeleteHotelCommand request, CancellationToken cancellationToken)
    {
        var hotel = await _hotelRepository.GetHotelByIdAsync(request.HotelId) ??
            throw new NotFoundException("The requested hotel not found");
        await _hotelRepository.DeleteHotelByIdAsync(request.HotelId);

    }
}

