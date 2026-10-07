using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HotelBooking.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddEndpointsApiExplorer(); //add any available endpoints to the API explorer
builder.Services.AddSwaggerGen(); //add swagger generator to the service collection
builder.Services.AddControllers(); //add controllers to the service collection
builder.Services.AddScoped<IHotelBookingRepository, HotelBookingRepository>(); //add the repository to the service collection
builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(
                        new JsonStringEnumConverter()
                    );
                });



string? tokenKeyString = builder.Configuration.GetSection("AppSettings:TokenKey").Value;
 
        SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                tokenKeyString != null ? tokenKeyString : ""
            ));


TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
{
    IssuerSigningKey = tokenKey,
    ValidateIssuer = false,
    ValidateIssuerSigningKey = false,
    ValidateAudience = false
};

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => 
                {
                    options.TokenValidationParameters = tokenValidationParameters;
                });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); //enable swagger middleware
    app.UseSwaggerUI(); //enable swagger UI middleware
}
else
{
    app.UseHttpsRedirection();
}



app.UseAuthentication();

app.UseAuthorization();

app.MapControllers(); //map controller endpoints to the app

app.Run();


