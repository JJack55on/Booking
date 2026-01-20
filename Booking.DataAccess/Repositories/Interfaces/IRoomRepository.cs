using Booking.DataAccess.Entities;

namespace Booking.DataAccess.Repositories;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(int id);
    Task<List<Room>> GetAllAsync();
    Task<Room> CreateAsync(Room room);
    Task DeleteAsync(Room room);
    Task SaveChangesAsync();
}