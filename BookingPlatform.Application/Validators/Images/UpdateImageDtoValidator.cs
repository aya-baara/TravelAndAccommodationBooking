using BookingPlatform.Application.Dtos.Images;
using FluentValidation;

namespace BookingPlatform.Application.Validators.Images;

public class UpdateImageDtoValidator : AbstractValidator<UpdateImageDto>
{
    public UpdateImageDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Image ID is required.");

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
