using AutoMapper;
using Booking.Application.DTOs;
using Booking.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;

        public RoomsController(IRoomService roomService, IMapper mapper)
        {
            _roomService = roomService;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetAll()
        {
            var rooms = await _roomService.GetAllRoomsAsync();
            var result = _mapper.Map<IEnumerable<RoomDto>>(rooms);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<RoomDto>> GetById(int id)
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            
            if (room == null)
                return NotFound();

            var result = _mapper.Map<RoomDto>(room);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoomDto>> Create([FromBody] CreateRoomDto dto)
        {
            var createRoomDto = _mapper.Map<CreateRoomDto>(dto);
            var createdRoom = await _roomService.CreateRoomAsync(createRoomDto);
            var result = _mapper.Map<RoomDto>(createdRoom);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _roomService.DeleteRoomAsync(id);
            
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("{id}/availability")]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> CheckAvailability(int id)
        {
            var isAvailable = await _roomService.IsRoomAvailableAsync(id);
            return Ok(isAvailable);
        }
    }