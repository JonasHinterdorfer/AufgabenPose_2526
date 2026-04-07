namespace WebAPI.Validators;

using FluentValidation;

using WebAPI.Endpoints;

public class CustomerDtoValidator: AbstractValidator<CustomerDto>
{
    public CustomerDtoValidator()
    {
        RuleFor(x => x.LastName)
            .MinimumLength(3)
            .WithMessage("LastName must be at least 3 characters long");

        RuleFor(x => x.FirstName)
            .EmailAddress()
            .WithMessage("First name must be a valid email address");
    }
}