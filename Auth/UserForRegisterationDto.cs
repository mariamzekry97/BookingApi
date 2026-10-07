using HotelBooking.Enums;

namespace HotelBooking.Dtos
{
    public class UserForRegisterationDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Roles Role { get; set; }
        public bool IsActive { get; set; }


        public UserForRegisterationDto()
        {
            if(Email == null)
            {
                Email = string.Empty;
            }
            if(Password == null)
            {
                Password = string.Empty;
            }
            if(PasswordConfirm == null)
            {
                PasswordConfirm = string.Empty;
            }
            if(FirstName == null)
            {
                FirstName = string.Empty;
            }
            if(LastName == null)
            {
                LastName = string.Empty;
            }
        }
    }
}