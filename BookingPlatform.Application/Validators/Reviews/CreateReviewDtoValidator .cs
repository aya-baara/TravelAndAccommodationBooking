using BookingPlatform.Application.Dtos.Reviews;
using FluentValidation;

namespace BookingPlatform.Application.Validators.Reviews;

public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDto>
{
    public CreateReviewDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.HotelId)
            .NotEmpty().WithMessage("Hotel ID is required.");

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Comment is required.")
            .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters.");

        RuleFor(x => x.Rate)
            .InclusiveBetween(1.0, 5.0).WithMessage("Rate must be between 1.0 and 5.0.");
    }
}
