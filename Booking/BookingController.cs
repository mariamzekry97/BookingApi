using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using HotelBooking.Data;
using HotelBooking.Models;
using HotelBooking.Dtos;
using HotelBooking.Enums;
namespace HotelBooking.Controllers;

[ApiController] //send and recieve data in JSON format, and automatically handle model validation errors
[Route("[controller]")] //define the route for the controller, which will be based on the controller's name (in this case, "Booking")
public class BookingController : ControllerBase
{
    IHotelBookingRepository _repository;
    IMapper _mapper;    
    public BookingController(IConfiguration config, IHotelBookingRepository repository)
    {
        _repository = repository;

         _mapper = new Mapper(new MapperConfiguration(cfg => {
            cfg.CreateMap<BookingDto, Booking>();
        }));
        
        _mapper = new Mapper(new MapperConfiguration(cfg =>{
            cfg.CreateMap<RoomBooking, RoomBookingDto>()
                .ForMember(d => d.GuestFirstName, o => o.MapFrom(s => s.Booking.GuestFirstName))
                .ForMember(d => d.GuestLastName, o => o.MapFrom(s => s.Booking.GuestLastName))
                .ForMember(d => d.CheckInDate, o => o.MapFrom(s => s.Booking.CheckInDate))
                .ForMember(d => d.CheckOutDate, o => o.MapFrom(s => s.Booking.CheckOutDate))
                .ForMember(d => d.NumberOfAdults, o => o.MapFrom(s => s.Booking.NumberOfAdults))
                .ForMember(d => d.NumberOfChildren, o => o.MapFrom(s => s.Booking.NumberOfChildren))
                .ForMember(d => d.RoomNumber, o => o.MapFrom(s => s.Room.Number))
                .ForMember(d => d.RoomPrice, o => o.MapFrom(s => s.Room.Price));
            }));
    }

    [HttpGet("bookings/all", Name = "GetBookings")] //define the route for the action method, which will be based on the controller's route and the action method's name (in this case, "bookings")
    public IEnumerable<Booking> GetBookings()
    {
        return _repository.GetBookings();
    }

    [HttpGet("bookings/{bookingId}", Name = "GetBooking")] //define the route for the action method, which will be based on the controller's route and the action method's name (in this case, "bookings")
    public ActionResult<RoomBookingDto> GetBookingById(int bookingId)
    {          
        RoomBookingDto bookingDto = _mapper.Map<RoomBookingDto>(_repository.GetBookingById(bookingId).Value);
        return bookingDto;
    }


    [HttpGet("bookings/available", Name = "GetAvailableRoom")] //define the route for the action method, which will be based on the controller's route and the action method's name (in this case, "bookings")
    public ActionResult<Room> GetAvailableRoom([FromQuery]BookingDto bookingDto)
    {
        return _repository.GetAvailableRoom(bookingDto);
    }
   
   
   
   [HttpPost("bookings", Name = "CreateBooking")] //define the route for the action method, which will be based on the controller's route and the action method's name (in this case, "bookings")
    public IActionResult CreateBooking(BookingDto bookingDto)
    {
        //Booking booking = _mapper.Map<Booking>(bookingDto);
        Booking booking = new Booking
        {
            GuestFirstName = bookingDto.GuestFirstName,
            GuestLastName = bookingDto.GuestLastName,
            CheckInDate = bookingDto.CheckInDate,
            CheckOutDate = bookingDto.CheckOutDate,
            NumberOfAdults = bookingDto.NumberOfAdults,
            NumberOfChildren = bookingDto.NumberOfChildren
        };
        _repository.AddEntity<Booking>(booking);
        _repository.AddEntity<RoomBooking>(new RoomBooking
        {
            Booking = booking,
            RoomId = bookingDto.RoomId,
            Status = BookingStatus.Booked
        });

        if(_repository.SaveChanges())
        {
            return Ok();
        }

        return BadRequest("Failed to create room.");
    }
}