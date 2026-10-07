using HotelBooking.Models;
using HotelBooking.Enums;

namespace HotelBooking.Dtos;
public class RoomBookingDto
{
    public string? RoomNumber { get; set; } = null!;
    public decimal? RoomPrice { get; set; }
    public string GuestFirstName { get; set; } = string.Empty;
    public string GuestLastName { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfAdults { get; set; }
    public int? NumberOfChildren { get; set; }

    public BookingStatus Status { get; set; }
}
