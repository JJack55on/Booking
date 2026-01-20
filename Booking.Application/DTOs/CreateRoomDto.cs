namespace Booking.Application.DTOs;

public class CreateRoomDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
}