using FluentValidation;

namespace WebAPI.Validators;

using WebAPI.Endpoints;

public class CustomerDtoValidator : AbstractValidator<CustomerDto>
{
    public CustomerDtoValidator()
    {
        RuleFor(x => x.LastName).NotEmpty().MinimumLength(3);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
    }
}
