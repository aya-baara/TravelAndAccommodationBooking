using AutoMapper;
using BookingPlatform.Application.Hotels.Dtos;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Interfaces.Repositories;
using MediatR;

namespace BookingPlatform.Application.Hotels.Commands.Create;

public class CreateHotelCommandHandler : IRequestHandler<CreateHotelCommand, HotelResponseDto>
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IMapper _mapper;
    public CreateHotelCommandHandler(IHotelRepository hotelRepository, IMapper mapper)
    {
        _hotelRepository = hotelRepository;
        _mapper = mapper;
    }

    public async Task<HotelResponseDto> Handle(CreateHotelCommand request, CancellationToken cancellationToken)
    {
        var hotel = _mapper.Map<Hotel>(request);
        var created =await  _hotelRepository.CreateHotelAsync(hotel);
        return _mapper.Map<HotelResponseDto>(created);
    }
}

