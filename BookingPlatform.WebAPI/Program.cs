using BookingPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using BookingPlatform.Infrastructure.Services;
using BookingPlatform.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BookingPlatform.Core.Interfaces.Auth;
using Serilog;
using BookingPlatform.WebAPI.Middlewares;
using FluentValidation.AspNetCore;
using FluentValidation;
using BookingPlatform.WebAPI.Extensions;
using System.Text.Json.Serialization;
using BookingPlatform.WebAPI.Helpers;
using BookingPlatform.Application.Mapping;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();

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
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); // optional
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    });
builder.Services.AddSwaggerGen(options =>
{
    // Add Swagger document info
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MinimalAPI",
        Version = "v1"
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // JWT Authentication setup for Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.\n\nExample: Bearer eyJhbGciOiJIUzI1NiIsInR..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddAutoMapper(typeof(OwnerProfile).Assembly);
builder.Services.AddFluentValidationAutoValidation(); // Auto model validation
builder.Services.AddValidatorsFromAssemblyContaining<Program>(); // Scan validators

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

builder.Host.UseSerilog();
var app = builder.Build();

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.MapControllers();
app.Run();

