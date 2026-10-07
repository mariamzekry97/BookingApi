namespace HotelBooking.Models;
public class Room
{
    public int Id { get; set; }
    public string? Number { get; set; }
    public int AdultsCapacity { get; set; }
    public int? ChildrenCapacity { get; set; }
    public decimal Price { get; set; }
    public ICollection<RoomBooking> RoomBookings { get; set; } = new List<RoomBooking>();

    //public ICollection<RoomAmenity> RoomAmenities { get; set; } = new List<RoomAmenity>();
    //public ICollection<RoomService> RoomServices { get; set; } = new List<RoomService>();
}
