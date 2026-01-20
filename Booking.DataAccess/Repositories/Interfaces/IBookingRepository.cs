namespace Booking.DataAccess.Repositories;

public interface IBookingRepository
{
    Task<Entities.Booking?> GetByIdAsync(int id);
    Task<List<Entities.Booking>> GetByUserIdAsync(string userId);
    Task<Entities.Booking> CreateAsync(Entities.Booking booking);
    Task SaveChangesAsync();
}