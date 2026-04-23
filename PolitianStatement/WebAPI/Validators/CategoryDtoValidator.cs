namespace WebAPI.Validators;

using FluentValidation;

using WebAPI.Endpoints;

public class CategoryDtoValidator : AbstractValidator<CategoryDto>
{
    public CategoryDtoValidator()
    {
        RuleFor(x => x.Description)
            .MinimumLength(3)
            .WithMessage("CategoryName must be at least 3 characters long");
    }
}