using Microsoft.AspNetCore.Mvc;
using HotelBooking.Models;
using HotelBooking.Dtos;

namespace HotelBooking.Data
{
    public interface IHotelBookingRepository
    {
        bool SaveChanges();
        void AddEntity<T>(T entity);
        void RemoveEntity<T>(T entity);
        public IEnumerable<Room> GetRooms();
        public ActionResult<Room> GetRoomById(int roomId);
        public IEnumerable<Booking> GetBookings();
        public ActionResult<RoomBooking> GetBookingById(int bookingId);
        public ActionResult<Room> GetAvailableRoom(BookingDto bookingDto);
    }
}   