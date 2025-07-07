using BookingPlatform.Core.Interfaces.Services;
using BookingPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using BookingPlatform.Infrastructure.Services;
using BookingPlatform.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BookingPlatform.Core.Interfaces.Auth;
using Serilog;
using BookingPlatform.Application.Services.Helpers;
using BookingPlatform.Application.SieveConfigurations;
using Sieve.Services;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Infrastructure.Repositories;
using BookingPlatform.Core.Interfaces;
using BookingPlatform.Infrastructure;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Application.Services.Commands;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Application.Services.Queries;


var builder = WebApplication.CreateBuilder(args);
// Load config from appsettings.json
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();

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
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWTToken"));
builder.Services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();

builder.Services.AddAuthorization();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<IPdfService, PdfGeneratorService>();


builder.Services.AddScoped<ISieveProcessor, SieveProcessor>();
// Register all custom Sieve configurations
builder.Services.AddScoped<ISieveCustomConfiguration, HotelSieveConfiguration>();
builder.Services.AddScoped<ISieveCustomConfiguration, CitySieveConfiguration>();
builder.Services.AddScoped<ISieveCustomConfiguration, RoomSieveConfiguration>();
builder.Services.AddScoped<ISieveCustomConfiguration, HotelManagementSieveConfiguration>();

// Register the composite config as the only ISieveConfiguration
builder.Services.AddScoped<ISieveConfiguration, CompositeSieveConfiguration>();

builder.Services.AddScoped<IBookingNotificationService, BookingNotificationService>();
builder.Services.AddScoped<IBookingCreationService, BookingCreationService>();
builder.Services.AddScoped<IBookingHtmlBuilder, BookingHtmlBuilder>();
builder.Services.AddScoped<IInvoiceHtmlBuilder, InvoiceHtmlBuilder>();

builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IInvoiceRopsitory, InvoiceRepository>();
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IBookingCommandService, BookingCommandService>();
builder.Services.AddScoped<IBookingQueryService, IBookingQueryService>();

builder.Services.AddScoped<ICityCommandService, CityCommandService>();
builder.Services.AddScoped<ICityQueryService, CityQueryService>();

builder.Services.AddScoped<IDiscountCommandService, DiscountCommandService>();
builder.Services.AddScoped<IDiscountQueryService, DiscountQueryService>();

builder.Services.AddScoped<IHotelCommandService, HotelCommandService>();
builder.Services.AddScoped<IHotelQueryService, HotelQueryService>();

builder.Services.AddScoped<IImageCommandService, ImageCommandService>();
builder.Services.AddScoped<IImageQueryService, ImageQueryService>();

builder.Services.AddScoped<IInvoiceCommandService, InvoiceCommandService>();
builder.Services.AddScoped<IInvoiceQueryService, InvoiceQueryService>();

builder.Services.AddScoped<IOwnerCommandService, OwnerCommandService>();
builder.Services.AddScoped<IOwnerQueryService, OwnerQueryService>();

builder.Services.AddScoped<IReviewCommandService, ReviewCommandService>();
builder.Services.AddScoped<IReviewQueryService, ReviewQueryService>();

builder.Services.AddScoped<IRoomCommandService, RoomCommandService>();
builder.Services.AddScoped<IRoomQueryService, RoomQueryService>();

builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();

builder.Services.AddScoped<IRoleCommandService, RoleCommandService>();
builder.Services.AddScoped<IRoleQueryService, RoleQueryService>();


builder.Host.UseSerilog();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.Run();

