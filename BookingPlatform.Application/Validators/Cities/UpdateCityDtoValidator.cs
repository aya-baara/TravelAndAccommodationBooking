using BookingPlatform.Application.Dtos.Cities;
using FluentValidation;

namespace BookingPlatform.Application.Validators.Cities;

public class UpdateCityDtoValidator : AbstractValidator<UpdateCityDto>
{
    public UpdateCityDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("City name is required.")
            .MaximumLength(100).WithMessage("City name must not exceed 100 characters.");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required.")
            .MaximumLength(100).WithMessage("Country name must not exceed 100 characters.");

        RuleFor(x => x.PostOffice)
            .NotEmpty().WithMessage("Post office is required.")
            .MaximumLength(50).WithMessage("Post office name must not exceed 50 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(300).WithMessage("Description must not exceed 300 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}

