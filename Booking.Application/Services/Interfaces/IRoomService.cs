using Booking.Application.DTOs;

namespace Booking.Application.Services;

public interface IRoomService
{
    Task<IEnumerable<RoomDto>> GetAllRoomsAsync();
    Task<RoomDto?> GetRoomByIdAsync(int id);
    Task<RoomDto> CreateRoomAsync(CreateRoomDto roomDto);
    Task<bool> DeleteRoomAsync(int id);
    Task<bool> IsRoomAvailableAsync(int roomId);
}