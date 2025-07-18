using BookingPlatform.Application.Dtos.Owners;
using FluentValidation;

namespace BookingPlatform.Application.Validators.Owners;

class UpdateOwnerDtoValidator : AbstractValidator<UpdateOwnerDto>
{
    public UpdateOwnerDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

        RuleFor(x => x.PhoneNumber)
            .InclusiveBetween(100000000, 999999999).WithMessage("Phone number must be a valid 9-digit number.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .LessThan(DateOnly.FromDateTime(DateTime.Today)).WithMessage("Date of birth must be in the past.");

    }
}


