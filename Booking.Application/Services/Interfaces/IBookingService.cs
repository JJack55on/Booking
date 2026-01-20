using Booking.Application.DTOs;

namespace Booking.Application.Services;

public interface IBookingService
{
    Task<BookingDto> CreateBookingAsync(CreateBookingDto bookingDto, string userId);
    Task<BookingDto?> GetBookingByIdAsync(int id, string userId);
    Task<IEnumerable<BookingDto>> GetUserBookingsAsync(string userId);
}