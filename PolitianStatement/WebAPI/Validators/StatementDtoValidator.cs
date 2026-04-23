namespace WebAPI.Validators;

using FluentValidation;

using WebAPI.Endpoints;

public class StatementDtoValidator : AbstractValidator<StatementDto>
{
    public StatementDtoValidator()
    {
        //RuleFor(x => x.)
        //    .MinimumLength(max)
        //    .WithMessage("Description must be at least max characters long");

    }
}