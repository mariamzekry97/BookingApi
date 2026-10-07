using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBooking.Models;
using HotelBooking.Dtos;
using HotelBooking.Enums;

namespace HotelBooking.Data;

public class HotelBookingRepository: IHotelBookingRepository
{
    private readonly DataContextEF _context;

    public HotelBookingRepository(IConfiguration config)
    {
        _context = new DataContextEF(config);
    }
    public bool SaveChanges()
    {
        return _context.SaveChanges() > 0;
    }

    public void AddEntity<T>(T entity) 
    {
        if(entity != null)
        {
            _context.Add(entity);
        }
    }

    public void RemoveEntity<T>(T entity) 
    {
        if(entity != null)
        {
            _context.Remove(entity);
        }
    }

    public IEnumerable<Room> GetRooms()
    {
        IEnumerable<Room> rooms = _context.Rooms.ToList();
        return rooms;
    }

    public ActionResult<Room> GetRoomById(int roomId)
    {
        Room? room = _context.Rooms.FirstOrDefault(r => r.Id == roomId);
        if(room == null)
        {
            throw new Exception("Room not found.");
        }
        
        return room;
    }

    public IEnumerable<Booking> GetBookings()
    {
        IEnumerable<Booking> bookings = _context.Bookings.ToList();
        return bookings;
    }

    public ActionResult<RoomBooking> GetBookingById(int bookingId)
    {
        var roomBooking = _context.RoomBookings
            .Include(rb => rb.Booking)
            .Include(rb => rb.Room)
            .FirstOrDefault(rb => rb.BookingId == bookingId);

        if (roomBooking == null)
        {
            throw new Exception("Booking not found.");
        }
        
        return roomBooking;
    }


    public ActionResult<Room> GetAvailableRoom(BookingDto booking)
    {
        Room? room = _context.Rooms
        .Where(room =>
            booking.NumberOfAdults > 0
            // Room can accommodate the adults
            && room.AdultsCapacity >= booking.NumberOfAdults

            // Room can accommodate everyone
            && room.AdultsCapacity + room.ChildrenCapacity
                >= booking.NumberOfAdults + booking.NumberOfChildren

            // No conflicting booking exists
            && !room.RoomBookings.Any(roomBooking =>
                roomBooking.Booking.CheckInDate.AddDays(1) <= booking.CheckOutDate
                && roomBooking.Booking.CheckOutDate.AddDays(-1) >= booking.CheckInDate
                && roomBooking.Status != BookingStatus.Canceled
            )
        )
        .FirstOrDefault();

        if(room == null)
        {
            throw new Exception("No available room found for the given booking details.");
        }

        return room;
    }
}
