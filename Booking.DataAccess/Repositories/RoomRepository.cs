using System.Data.Entity;
using Booking.DataAccess.Context;
using Booking.DataAccess.Entities;

namespace Booking.DataAccess.Repositories;

public class RoomRepository :IRoomRepository
{
    private readonly BookingDbContext _context;

    public RoomRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<Room?> GetByIdAsync(int id)
    {
        return await _context.Rooms
            .Include(r => r.Bookings)
            .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);
    }

    public async Task<List<Room>> GetAllAsync()
    {
        return await _context.Rooms
            .Where(r => r.IsActive)
            .ToListAsync();
    }

    public async Task<Room> CreateAsync(Room room)
    {
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();
        return room;
    }

    public async Task DeleteAsync(Room room)
    {
        var hasBookings = await _context.Bookings
            .AnyAsync(b => b.RoomId == room.Id);

        if (hasBookings)
        {
            room.IsActive = false;
        }
        else
        {
            _context.Rooms.Remove(room);
        }
            
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}