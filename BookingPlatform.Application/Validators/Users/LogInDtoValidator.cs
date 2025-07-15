using BookingPlatform.Application.Dtos.Users;
using FluentValidation;

namespace BookingPlatform.Application.Validators.Users;

public class LogInDtoValidator : AbstractValidator<LogInDto>
{
    public LogInDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be valid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
            .MaximumLength(100).WithMessage("Password must not exceed 100 characters.");
    }
}

