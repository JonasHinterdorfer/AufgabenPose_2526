namespace WebAPI.Validators;

using FluentValidation;

using WebAPI.Endpoints;

public class RatingDtoValidator : AbstractValidator<RatingDto>
{
    public RatingDtoValidator()
    {
        RuleFor(x => x.Rate)
            .InclusiveBetween(1, 5)
            .WithMessage("Rate must be between 1 and 5");

        RuleFor(x => x.UserName)
            .MinimumLength(2)
            .WithMessage("UserName must be at least 2 characters long");
    }
}
