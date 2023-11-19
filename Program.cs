using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using LostButFound.API.DAL;
using LostButFound.API.DAL.Interfaces;
using LostButFound.API.DAL.Repositories;
using LostButFound.API.Services.Implementations;
using LostButFound.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connection = builder.Configuration.GetConnectionString("LostButFoundConnectionString");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connection));


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    var secret = builder.Configuration.GetValue<string>("Secret");
    var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
    x.RequireHttpsMetadata = true;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = "https://localhost:7000/",
        ValidIssuer = "https://localhost:7000/",
        IssuerSigningKey = key,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
}); 




builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddSwaggerGen();

//builder.Services.AddHttpContextAccessor();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
{
    build.WithOrigins("http://192.168.31.64:5173").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("corspolicy");

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
