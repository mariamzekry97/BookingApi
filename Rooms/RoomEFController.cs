using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using HotelBooking.Data;
using HotelBooking.Models;
using HotelBooking.Dtos;
namespace HotelBooking.Controllers;

[ApiController] //send and recieve data in JSON format, and automatically handle model validation errors
[Route("[controller]")] //define the route for the controller, which will be based on the controller's name (in this case, "Booking")
public class RoomEFController : ControllerBase
{
    //DataContextEF _entityFramework;
    IHotelBookingRepository _repository;
    IMapper _mapper;    
    public RoomEFController(IConfiguration config, IHotelBookingRepository repository)
    {
        //_entityFramework = new DataContextEF(config);
        _repository = repository;
        _mapper = new Mapper(new MapperConfiguration(cfg => {
            cfg.CreateMap<RoomDto, Room>();
        }));
    }
    [HttpGet("rooms", Name = "GetRoomsEF")] //define the route for the action method, which will be based on the controller's route and the action method's name (in this case, "rooms")
    public IEnumerable<Room> GetRooms()
    {
        return _repository.GetRooms();
    }

    [HttpGet("rooms/{roomId}", Name = "GetRoomEF")] //define the route for the action method, which will be based on the controller's route and the action method's name (in this case, "rooms")
    public ActionResult<Room> GetRoomById(int roomId)
    {          
        return _repository.GetRoomById(roomId);
    }


    [HttpPost("rooms", Name = "CreateRoomEF")] //define the route for the action method, which will be based on the controller's route and the action method's name (in this case, "rooms")
    public IActionResult CreateRoom(RoomDto roomDto)
    {
        if(string.IsNullOrEmpty(roomDto.Number))
        {
            return BadRequest("Room number is required.");
        }
        if(roomDto.Price <= 0)
        {
            return BadRequest("Room price must be greater than zero.");
        }
        if(roomDto.AdultsCapacity <= 0)
        {
            return BadRequest("Room must have at least one adult capacity.");
        }
        if(roomDto.ChildrenCapacity < 0)
        {
            return BadRequest("Room children capacity cannot be negative.");
        }

        Room room = _mapper.Map<Room>(roomDto);

        _repository.AddEntity<Room>(room);

        if(_repository.SaveChanges())
        {
            return Ok();
        }

        return BadRequest("Failed to create room.");
    }


    [HttpPut("EditRoom", Name = "EditRoomEF")] //define the route for the action method, which will be based on the controller's route and the action method's name (in this case, "rooms")
    public ActionResult EditRoom(Room room)
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

        if(room.Id <= 0)
        {
            return BadRequest("Invalid room ID.");
        }
        Room? roomDb = _repository.GetRoomById(room.Id).Value;
        if(roomDb == null)
        {
            return BadRequest("Room not found.");
        }
        roomDb.Number = room.Number;
        roomDb.Price = room.Price;
        roomDb.AdultsCapacity = room.AdultsCapacity;
        roomDb.ChildrenCapacity = room.ChildrenCapacity;    
        if(_repository.SaveChanges())
        {
            return Ok();
        }
        return BadRequest("Failed to edit room.");
    }


    [HttpDelete("DeleteRoom/{roomId}", Name = "DeleteRoomEF")] //define the route for the action method, which will be based on the controller's route and the action method's name (in this case, "rooms")
    public ActionResult DeleteRoom(int roomId)
    {
        if(roomId <= 0)
        {
            return BadRequest("Invalid room ID.");
        }
        Room? room = _repository.GetRoomById(roomId).Value;
        if(room == null)
        {
            return NotFound("Room not found.");
        }
        _repository.RemoveEntity<Room>(room);
        if(_repository.SaveChanges())
        {
            return Ok();
        }
        return BadRequest("Failed to delete room.");
    }
}