using HotelBooking.Enums;
namespace HotelBooking.Models;

public class RoomBooking
{
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;

    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;

    public BookingStatus Status { get; set; }
}
