using System.Data.Entity;
using Booking.DataAccess.Context;
using Booking.DataAccess.Entities;

namespace Booking.DataAccess.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly BookingDbContext _context;

    public BookingRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<Entities.Booking?> GetByIdAsync(int id)
    {
        return await _context.Bookings
            .Include(b => b.Room)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<List<Entities.Booking>> GetByUserIdAsync(string userId)
    {
        return await _context.Bookings
            .Include(b => b.Room)
            .Where(b => b.UserId == userId)
            .ToListAsync();
    }

    public async Task<Entities.Booking> CreateAsync(Entities.Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}