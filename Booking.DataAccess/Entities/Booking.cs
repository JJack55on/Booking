namespace Booking.DataAccess.Entities;

public class Booking
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;
    public DateTime BookingDate { get; set; }
}