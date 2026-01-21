using AutoMapper;
using Booking.Application.DTOs;
using Booking.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;

        public BookingsController(IBookingService bookingService, IMapper mapper)
        {
            _bookingService = bookingService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<BookingDto>> Create([FromBody] CreateBookingDto dto)
        {
            var userId = User.Identity?.Name ?? string.Empty;
            
            try
            {
                var createBookingDto = _mapper.Map<CreateBookingDto>(dto);
                var booking = await _bookingService.CreateBookingAsync(createBookingDto, userId);
                var result = _mapper.Map<BookingDto>(booking);

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetById(int id)
        {
            var userId = User.Identity?.Name ?? string.Empty;
            var booking = await _bookingService.GetBookingByIdAsync(id, userId);
            
            if (booking == null)
                return NotFound();

            var result = _mapper.Map<BookingDto>(booking);
            return Ok(result);
        }

        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetMyBookings()
        {
            var userId = User.Identity?.Name ?? string.Empty;
            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            var result = _mapper.Map<IEnumerable<BookingDto>>(bookings);

            return Ok(result);
        }
    }