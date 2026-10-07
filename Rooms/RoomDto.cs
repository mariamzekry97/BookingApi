namespace HotelBooking.Dtos;
public class RoomDto
{
    public string? Number { get; set; }
    public int AdultsCapacity { get; set; }
    public int? ChildrenCapacity { get; set; }
    public decimal Price { get; set; }
}
