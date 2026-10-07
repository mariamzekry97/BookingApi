using HotelBooking.Models;
using Microsoft.AspNetCore.Mvc;
using  HotelBooking.Data;
using Microsoft.AspNetCore.Authorization;
namespace HotelBooking.Controllers;



[Authorize]
[ApiController] //send and recieve data in JSON format, and automatically handle model validation errors
[Route("[controller]")] //define the route for the controller, which will be based on the controller's name (in this case, "Booking")
public class RoomController : ControllerBase
{
    DataContextDapper _dapper;
    public RoomController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("rooms", Name = "GetRooms")] //define the route for the action method, which will be based on the controller's route and the action method's name (in this case, "rooms")
    public IEnumerable<Room> GetRooms()
    {
        return _dapper.LoadData<Room>("SELECT * FROM Rooms");
    }

    [HttpGet("rooms/{roomId}", Name = "GetRoom")] //define the route for the action method, which will be based on the controller's route and the action method's name (in this case, "rooms")
    public ActionResult<Room> GetRoomById(int roomId)
    {
        if(roomId <= 0)
        {
            return BadRequest("Invalid room ID.");
        }
        Room? room = _dapper.LoadDataSingle<Room>($"SELECT * FROM Rooms WHERE Id = {roomId}");
        if(room == null)
        {
            return NotFound("Room not found.");
        }
        
        return Ok(_dapper.LoadDataSingle<Room>($"SELECT * FROM Rooms WHERE Id = {roomId}"));
    }


    [HttpPost("rooms", Name = "CreateRoom")] //define the route for the action method, which will be based on the controller's route and the action method's name (in this case, "rooms")
    public IActionResult CreateRoom(Room room)
    {
        if(string.IsNullOrEmpty(room.Number))
        {
            return BadRequest("Room number is required.");
        }
        if(room.Price <= 0)
        {
            return BadRequest("Room price must be greater than zero.");
        }
        if(room.AdultsCapacity <= 0)
        {
            return BadRequest("Room must have at least one adult capacity.");
        }
        if(room.ChildrenCapacity < 0)
        {
            return BadRequest("Room children capacity cannot be negative.");
        }

        if(_dapper.ExecuteSql($"INSERT INTO Rooms (Number, AdultsCapacity, ChildrenCapacity, Price) VALUES ('{room.Number}', {room.AdultsCapacity}, {room.ChildrenCapacity}, {room.Price})"))
        {
            return Ok();
        }

        return BadRequest("Failed to create room.");
    }


    [HttpPut("EditRoom")] //define the route for the action method, which will be based on the controller's route and the action method's name (in this case, "rooms")
    public IActionResult EditRoom(Room room)
    {
        if(string.IsNullOrEmpty(room.Number))
        {
            return BadRequest("Room number is required.");
        }
        if(room.Price <= 0)
        {
            return BadRequest("Room price must be greater than zero.");
        }
        if(room.AdultsCapacity <= 0)
        {
            return BadRequest("Room must have at least one adult capacity.");
        }
        if(room.ChildrenCapacity < 0)
        {
            return BadRequest("Room children capacity cannot be negative.");
        }

        if(_dapper.ExecuteSql($"UPDATE Rooms SET Number = '{room.Number}', AdultsCapacity = {room.AdultsCapacity}, ChildrenCapacity = {room.ChildrenCapacity}, Price = {room.Price} WHERE Id = {room.Id}"))
        {
            return Ok();
        }

        return BadRequest("Failed to edit room.");
    }
}