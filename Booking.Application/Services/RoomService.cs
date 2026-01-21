using AutoMapper;
using Booking.Application.DTOs;
using Booking.DataAccess.Entities;
using Booking.DataAccess.Repositories;
using Microsoft.Extensions.Logging;

namespace Booking.Application.Services;

    public class RoomService :IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomService> _logger;

        public RoomService(
            IRoomRepository roomRepository, 
            IMapper mapper,
            ILogger<RoomService> logger)
        {
            _roomRepository = roomRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<RoomDto>> GetAllRoomsAsync()
        {
            try
            {
                var rooms = await _roomRepository.GetAllAsync();
                var roomDtos = _mapper.Map<IEnumerable<RoomDto>>(rooms);
                
                foreach (var roomDto in roomDtos)
                {
                    var room = rooms.First(r => r.Id == roomDto.Id);
                    roomDto.IsBooked = room.Bookings.Any();
                }

                return roomDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all rooms");
                throw;
            }
        }

        public async Task<RoomDto?> GetRoomByIdAsync(int id)
        {
            try
            {
                var room = await _roomRepository.GetByIdAsync(id);
                
                if (room == null)
                {
                    _logger.LogWarning("Room with id {RoomId} not found", id);
                    return null;
                }

                var roomDto = _mapper.Map<RoomDto>(room);
                roomDto.IsBooked = room.Bookings.Any();
                
                return roomDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting room by id {RoomId}", id);
                throw;
            }
        }

        public async Task<RoomDto> CreateRoomAsync(CreateRoomDto roomDto)
        {
            try
            {
                var room = _mapper.Map<Room>(roomDto);
                var createdRoom = await _roomRepository.CreateAsync(room);
                
                _logger.LogInformation("Room created with id {RoomId}", createdRoom.Id);
                
                var result = _mapper.Map<RoomDto>(createdRoom);
                result.IsBooked = false;
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating room");
                throw;
            }
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            try
            {
                var room = await _roomRepository.GetByIdAsync(id);
                
                if (room == null)
                {
                    _logger.LogWarning("Room with id {RoomId} not found for deletion", id);
                    return false;
                }

                await _roomRepository.DeleteAsync(room);
                _logger.LogInformation("Room with id {RoomId} deleted", id);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting room with id {RoomId}", id);
                throw;
            }
        }

        public async Task<bool> IsRoomAvailableAsync(int roomId)
        {
            try
            {
                var room = await _roomRepository.GetByIdAsync(roomId);
                return room != null && !room.Bookings.Any();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking room availability for id {RoomId}", roomId);
                throw;
            }
        }
    }