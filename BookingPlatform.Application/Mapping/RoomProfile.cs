using AutoMapper;
using BookingPlatform.Application.Dtos.Rooms;
using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Mapping;

public class RoomProfile : Profile
{
    public RoomProfile()
    {
        CreateMap<CreateRoomDto, Room>();
        CreateMap<UpdateRoomDto, Room>();
        CreateMap<Room, RoomResponseDto>();
    }
}

