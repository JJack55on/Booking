namespace Booking.Application.DTOs;

public class RoomDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsBooked { get; set; }
}