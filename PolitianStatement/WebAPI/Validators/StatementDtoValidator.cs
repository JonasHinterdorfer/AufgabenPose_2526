namespace WebAPI.Validators;

using FluentValidation;

using WebAPI.Endpoints;

public class StatementDtoValidator : AbstractValidator<StatementDto>
{
    public StatementDtoValidator()
    {
        RuleFor(x => x.Description)
            .MinimumLength(10)
            .WithMessage("Description must be at least 10 characters long");
    }
}