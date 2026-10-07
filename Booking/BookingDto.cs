namespace HotelBooking.Models;
public class BookingDto
{
    public string GuestFirstName { get; set; } = string.Empty;
    public string GuestLastName { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfAdults { get; set; }
    public int? NumberOfChildren { get; set; }
    public int RoomId { get; set; }
}
