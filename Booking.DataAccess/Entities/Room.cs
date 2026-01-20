namespace Booking.DataAccess.Entities;

public class Room
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
    public List<Booking> Bookings { get; set; } = new();
}