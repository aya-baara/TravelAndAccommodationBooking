using AutoMapper;
using BookingPlatform.Application.Hotels.Dtos;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using MediatR;

namespace BookingPlatform.Application.Hotels.Commands.Create;

public class CreateHotelCommandHandler : IRequestHandler<CreateHotelCommand, HotelResponseDto>
{
    private readonly IHotelRepository _hotelRepository;
    private readonly ICityRepository _cityRepository;
    private readonly IOwnerRepository _ownerRepository;
    private readonly IMapper _mapper;
    public CreateHotelCommandHandler(IHotelRepository hotelRepository, IMapper mapper, ICityRepository cityRepository, IOwnerRepository ownerRepository)
    {
        _hotelRepository = hotelRepository;
        _mapper = mapper;
        _cityRepository = cityRepository;
        _ownerRepository = ownerRepository;
    }

    public async Task<HotelResponseDto> Handle(CreateHotelCommand request, CancellationToken cancellationToken)
    {
        var hotel = _mapper.Map<Hotel>(request);
        var city = await _cityRepository.GetCityByIdAsync(request.CityId)
            ?? throw new NotFoundException($"City with id {request.CityId} not found"); 

        var owner = await _ownerRepository.GetOwnerByIdAsync(request.OwnerId)
            ?? throw new NotFoundException($"Owner with id {request.OwnerId} not found");

        var created =await  _hotelRepository.CreateHotelAsync(hotel);
        return _mapper.Map<HotelResponseDto>(created);
    }
}

