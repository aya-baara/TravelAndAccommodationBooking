using BookingPlatform.Application.Dtos.Images;
using FluentValidation;

namespace BookingPlatform.Application.Validators.Images;

public class CreateImageDtoValidator : AbstractValidator<CreateImageDto>
{
    public CreateImageDtoValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid image type provided.");

        RuleFor(x => x.Path)
            .NotEmpty().WithMessage("Image path is required.")
            .MaximumLength(500).WithMessage("Image path must not exceed 500 characters.")
            .Must(path => Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute))
            .WithMessage("Path must be a valid URI.");
    }
}
