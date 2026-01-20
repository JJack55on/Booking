using AutoMapper;
using Booking.Application.DTOs;
using Booking.DataAccess.Repositories;
using Booking.DataAccess.Entities;
using Microsoft.Extensions.Logging;

namespace Booking.Application.Services;

public class BookingService : IBookingService
{
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BookingService> _logger;

        public BookingService(
            IBookingRepository bookingRepository,
            IRoomRepository roomRepository,
            IMapper mapper,
            ILogger<BookingService> logger)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BookingDto> CreateBookingAsync(CreateBookingDto bookingDto, string userId)
        {
            try
            {
                var room = await _roomRepository.GetByIdAsync(bookingDto.RoomId);
                
                if (room == null)
                {
                    _logger.LogWarning("Room with id {RoomId} not found for booking", bookingDto.RoomId);
                    throw new KeyNotFoundException($"Room with id {bookingDto.RoomId} not found");
                }
                
                if (room.Bookings.Any())
                {
                    _logger.LogWarning("Room with id {RoomId} is already booked", bookingDto.RoomId);
                    throw new InvalidOperationException($"Room with id {bookingDto.RoomId} is already booked");
                }
                
                var booking = _mapper.Map<DataAccess.Entities.Booking>(bookingDto);
                booking.UserId = userId;
                booking.BookingDate = DateTime.UtcNow;

                var createdBooking = await _bookingRepository.CreateAsync(booking);
                
                _logger.LogInformation("Booking created with id {BookingId} for user {UserId}", 
                    createdBooking.Id, userId);
                
                var result = _mapper.Map<BookingDto>(createdBooking);
                return result;
            }
            catch (Exception ex) when (ex is not KeyNotFoundException && ex is not InvalidOperationException)
            {
                _logger.LogError(ex, "Error creating booking for user {UserId}", userId);
                throw;
            }
        }

        public async Task<BookingDto?> GetBookingByIdAsync(int id, string userId)
        {
            try
            {
                var booking = await _bookingRepository.GetByIdAsync(id);
                
                if (booking == null)
                {
                    _logger.LogWarning("Booking with id {BookingId} not found", id);
                    return null;
                }
                
                if (booking.UserId != userId)
                {
                    _logger.LogWarning("User {UserId} attempted to access booking {BookingId} belonging to another user", 
                        userId, id);
                    return null;
                }

                var result = _mapper.Map<BookingDto>(booking);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting booking by id {BookingId} for user {UserId}", id, userId);
                throw;
            }
        }

        public async Task<IEnumerable<BookingDto>> GetUserBookingsAsync(string userId)
        {
            try
            {
                var bookings = await _bookingRepository.GetByUserIdAsync(userId);
                var result = _mapper.Map<IEnumerable<BookingDto>>(bookings);
                
                _logger.LogInformation("Retrieved {Count} bookings for user {UserId}", 
                    result.Count(), userId);
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bookings for user {UserId}", userId);
                throw;
            }
        }
    }