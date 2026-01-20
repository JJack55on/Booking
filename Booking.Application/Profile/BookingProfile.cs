using Booking.Application.DTOs;

namespace Booking.Application.Profile;

public class BookingProfile : AutoMapper.Profile
{
    public BookingProfile()
    {
        CreateMap<DataAccess.Entities.Booking, BookingDto>()
            .ForMember(dest => dest.RoomDescription, 
                opt => opt.MapFrom(src => src.Room.Description))
            .ForMember(dest => dest.RoomPrice, 
                opt => opt.MapFrom(src => src.Room.Price));
        
        CreateMap<CreateBookingDto, DataAccess.Entities.Booking>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.BookingDate, opt => opt.Ignore())
            .ForMember(dest => dest.Room, opt => opt.Ignore());
    }
}