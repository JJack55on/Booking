namespace Booking.Application.DTOs;

public class BookingDto
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string RoomDescription { get; set; } = string.Empty;
    public decimal RoomPrice { get; set; }
    public DateTime BookingDate { get; set; }
    public string UserId { get; set; } = string.Empty;
}