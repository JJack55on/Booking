using Booking.Application.DTOs;
using Booking.DataAccess.Entities;

namespace Booking.Application.Profile;

public class RoomProfile : AutoMapper.Profile
{
    public RoomProfile()
    {
        CreateMap<Room, RoomDto>()
            .ForMember(dest => dest.IsBooked, opt => opt.Ignore());
        
        CreateMap<CreateRoomDto, Room>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.Bookings, opt => opt.Ignore());
    }
}