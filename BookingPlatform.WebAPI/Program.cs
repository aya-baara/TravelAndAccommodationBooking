using BookingPlatform.Core.Interfaces.Services;
using BookingPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using BookingPlatform.Infrastructure.Services;
using BookingPlatform.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BookingPlatform.Core.Interfaces.Auth;


var builder = WebApplication.CreateBuilder(args);
// Load config from appsettings.json
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAuthentication(k =>
{
    k.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    k.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(p =>
{
    var key = Encoding.UTF8.GetBytes(builder.Configuration["JWTToken:key"]);
    p.SaveToken = true;
    p.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWTToken:key"],
        ValidAudience = builder.Configuration["JWTToken:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWTToken"));
builder.Services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();

builder.Services.AddAuthorization();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<IPdfService, PdfGeneratorService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.Run();

