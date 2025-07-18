using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Application.Services.Commands;
using BookingPlatform.Application.Services.Helpers;
using BookingPlatform.Application.Services.Queries;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Core.Interfaces;
using BookingPlatform.Infrastructure.Repositories;
using BookingPlatform.Infrastructure;
using BookingPlatform.Core.Interfaces.Services;
using Sieve.Services;
using BookingPlatform.Infrastructure.Services;

namespace BookingPlatform.WebAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IBookingCommandService, BookingCommandService>();
        services.AddScoped<IBookingQueryService, BookingQueryService>();

        services.AddScoped<ICityCommandService, CityCommandService>();
        services.AddScoped<ICityQueryService, CityQueryService>();

        services.AddScoped<IDiscountCommandService, DiscountCommandService>();
        services.AddScoped<IDiscountQueryService, DiscountQueryService>();

        services.AddScoped<IHotelCommandService, HotelCommandService>();
        services.AddScoped<IHotelQueryService, HotelQueryService>();

        services.AddScoped<IImageCommandService, ImageCommandService>();
        services.AddScoped<IImageQueryService, ImageQueryService>();

        services.AddScoped<IInvoiceCommandService, InvoiceCommandService>();
        services.AddScoped<IInvoiceQueryService, InvoiceQueryService>();

        services.AddScoped<IOwnerCommandService, OwnerCommandService>();
        services.AddScoped<IOwnerQueryService, OwnerQueryService>();

        services.AddScoped<IReviewCommandService, ReviewCommandService>();
        services.AddScoped<IReviewQueryService, ReviewQueryService>();

        services.AddScoped<IRoomCommandService, RoomCommandService>();
        services.AddScoped<IRoomQueryService, RoomQueryService>();

        services.AddScoped<IUserCommandService, UserCommandService>();
        services.AddScoped<IUserQueryService, UserQueryService>();

        services.AddScoped<IRoleCommandService, RoleCommandService>();
        services.AddScoped<IRoleQueryService, RoleQueryService>();

        services.AddScoped<ISieveProcessor, SieveProcessor>();

        services.AddScoped<IBookingNotificationService, BookingNotificationService>();
        services.AddScoped<IBookingCreationService, BookingCreationService>();
        services.AddScoped<IBookingHtmlBuilder, BookingHtmlBuilder>();
        services.AddScoped<IInvoiceHtmlBuilder, InvoiceHtmlBuilder>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddSingleton<IPdfService, PdfGeneratorService>();
        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<IDiscountRepository, DiscountRepository>();
        services.AddScoped<IHotelRepository, HotelRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<IInvoiceRopsitory, InvoiceRepository>();
        services.AddScoped<IOwnerRepository, OwnerRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}

