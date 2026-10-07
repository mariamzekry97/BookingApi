using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using HotelBooking.Data;
using HotelBooking.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace HotelBooking.Controllers;


[Authorize]
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
   
    private readonly DataContextDapper _dapper;
    private readonly IConfiguration _config;
    public AuthController(IConfiguration config)
    {
        _config = config;
        _dapper = new DataContextDapper(config);
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public IActionResult Register(UserForRegisterationDto userForRegisterationDto)
    {
        if(userForRegisterationDto.Password != userForRegisterationDto.PasswordConfirm)
        {
            return BadRequest("Passwords do not match.");
        }

        IEnumerable<String> existingUser = _dapper.LoadData<string>($"SELECT Email FROM [HotelBookingDb].[dbo].Auth WHERE Email = '{userForRegisterationDto.Email}'");
        if(existingUser.Count() > 0)
        {
            return BadRequest("User already exists.");
        }

        byte[] passwordSalt = new byte[128 / 8];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetNonZeroBytes(passwordSalt);
        }

        byte[] passwordHash = GetPasswordHash(userForRegisterationDto.Password, passwordSalt);
       
        
        string sqlAddAuth = @"INSERT INTO [HotelBookingDb].[dbo].Auth (Email, PasswordHash, PasswordSalt) VALUES ('" + userForRegisterationDto.Email + "', @PasswordHash, @PasswordSalt)";
        
        List<SqlParameter> parameters = new List<SqlParameter>();
        SqlParameter passwordSaltParam = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
        SqlParameter passwordHashParam = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
        passwordSaltParam.Value = passwordSalt;
        passwordHashParam.Value = passwordHash;
        parameters.Add(passwordSaltParam);
        parameters.Add(passwordHashParam);
        
       if( _dapper.ExecuteSqlWithParameters(sqlAddAuth, parameters))
        {
            string sqlAddUser = @"
                            INSERT INTO [HotelBookingDb].[dbo].Users(
                                [FirstName],
                                [LastName],
                                [Email],
                                [RoleId],
                                [Active]
                            ) VALUES (" +
                                "'" + userForRegisterationDto.FirstName + 
                                "', '" + userForRegisterationDto.LastName +
                                "', '" + userForRegisterationDto.Email + 
                                "', '" + (int)userForRegisterationDto.Role + 
                                "', 1)";
            if (!_dapper.ExecuteSql(sqlAddUser))
            {
                return BadRequest("Error occurred while adding user details.");
            }
            return Ok("User registered successfully.");
        }

        return BadRequest("User registration failed.");
    }


    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLoginDto)
    {
        string sqlForHashAndSalt = @"SELECT 
        [PASSWORDHASH], [PASSWORDSALT] FROM [HotelBookingDb].[dbo].Auth WHERE Email = '" + userForLoginDto.Email + "'";
        
        UserForLoginConfirmationDto? userForConfirmation = _dapper.LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);
        if (userForConfirmation == null)
        {
            return BadRequest("Invalid username or password");
        }
        
        byte[] passwordHash = GetPasswordHash(userForLoginDto.Password, userForConfirmation.PasswordSalt);
        
        for(int i = 0; i < passwordHash.Length; i++)
        {
            if(passwordHash[i] != userForConfirmation.PasswordHash[i])
            {
                return Unauthorized("Invalid credentials."); 
                //return StatusCode(401, "Invalid credentials."); //returning 401 Unauthorized status code with a message indicating that the credentials provided by the user are invalid.
            }
        }

        int userId = _dapper.LoadDataSingle<int>("SELECT Id FROM [HotelBookingDb].[dbo].Users WHERE Email = '" + userForLoginDto.Email + "'");
        return Ok(new Dictionary<string, string>
        {
            { "token", CreateToken(userId) }
        });
    }



    [HttpGet("RefreshToken")]
    public IActionResult RefreshToken()
    {
        string userId = User.FindFirst("userId")?.Value + "";
        int userIdDb = _dapper.LoadDataSingle<int>("SELECT Id FROM [HotelBookingDb].[dbo].Users WHERE Id = '" + userId + "'");
        
        
        return Ok(new Dictionary<string, string>
        {
            { "token", CreateToken(userIdDb) }
        });
    }




    private byte[] GetPasswordHash(string password, byte[] passwordSalt)
    {
        string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value +
            Convert.ToBase64String(passwordSalt);

        return KeyDerivation.Pbkdf2(
            password: password,
            salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 1000000,
            numBytesRequested: 256 / 8
        );
    }


    private string CreateToken(int userId)
    {
        // Implement token creation logic here
        Claim[] claims = new Claim[]
        {
            new Claim("userId", userId.ToString())
        };

        string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;
 
        SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                tokenKeyString != null ? tokenKeyString : ""
            ));


        SigningCredentials credentials = new SigningCredentials(
            tokenKey, 
            SecurityAlgorithms.HmacSha512Signature
        );

        SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = credentials
        };


        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken token = tokenHandler.CreateToken(descriptor);

        return tokenHandler.WriteToken(token);
    }
}