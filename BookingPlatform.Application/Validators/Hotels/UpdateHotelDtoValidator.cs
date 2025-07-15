using BookingPlatform.Application.Dtos.Hotels;
using FluentValidation;

namespace BookingPlatform.Application.Validators.Hotels;

public class UpdateHotelDtoValidator : AbstractValidator<UpdateHotelDto>
{
    public UpdateHotelDtoValidator()
    {
        RuleFor(x => x.Id)
           .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Hotel name is required.")
            .MaximumLength(100).WithMessage("Hotel name must not exceed 100 characters.");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required.")
            .MaximumLength(200).WithMessage("Location must not exceed 200 characters.");

        RuleFor(x => x.CityId)
            .NotEmpty().WithMessage("City is required.");

        RuleFor(x => x.StarRating)
            .InclusiveBetween(1, 5).WithMessage("Star rating must be between 1 and 5.");

        RuleFor(x => x.FullDescription)
            .NotEmpty().WithMessage("Full description is required.")
            .MaximumLength(2000).WithMessage("Full description must not exceed 2000 characters.");

        RuleFor(x => x.BriefDescription)
            .NotEmpty().WithMessage("Brief description is required.")
            .MaximumLength(500).WithMessage("Brief description must not exceed 500 characters.");

        RuleFor(x => x.PhoneNumber)
            .GreaterThan(0).WithMessage("Phone number must be a positive number.")
            .LessThanOrEqualTo(999999999).WithMessage("Phone number must be a valid number.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");

        RuleFor(x => x.OwnerId)
            .NotEmpty().WithMessage("Owner is required.");
    }
}

