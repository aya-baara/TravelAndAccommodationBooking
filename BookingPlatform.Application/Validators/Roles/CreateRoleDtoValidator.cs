using FluentValidation;
using BookingPlatform.Application.Dtos.Roles;
using BookingPlatform.Core.Enums;

namespace BookingPlatform.Application.Validators.Roles;
public class CreateRoleDtoValidator : AbstractValidator<CreateRoleDto>
{
    public CreateRoleDtoValidator()
    {
        RuleFor(x => x.Name)
            .IsInEnum().WithMessage("Role type must be one of the defined values: Admin, User, etc.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(200).WithMessage("Description must not exceed 200 characters.");
    }
}

