using BookingPlatform.Application.Dtos.Rooms;
using FluentValidation;

namespace BookingPlatform.Application.Validators.Rooms;

public class UpdateRoomDtoValidator : AbstractValidator<UpdateRoomDto>
{
    public UpdateRoomDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Room ID is required.");

        RuleFor(x => x.RoomType)
            .IsInEnum().WithMessage("Invalid room type.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        RuleFor(x => x.PricePerNight)
            .GreaterThan(0).WithMessage("Price per night must be greater than 0.");

        RuleFor(x => x.AdultCapacity)
            .GreaterThanOrEqualTo(1).WithMessage("Adult capacity must be at least 1.");

        RuleFor(x => x.ChildrenCapacity)
            .GreaterThanOrEqualTo(0).WithMessage("Children capacity cannot be negative.");

        RuleFor(x => x.HotelId)
            .NotEmpty().WithMessage("Hotel ID is required.");

    }
}

