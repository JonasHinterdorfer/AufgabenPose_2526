namespace WebAPI.Validators;

using FluentValidation;

using WebAPI.Endpoints;

public class RoomDtoValidator : AbstractValidator<RoomDto>
{
    public RoomDtoValidator()
    {
        RuleFor(x => x.RoomNumber)
            .MinimumLength(3)
            .WithMessage("RoomNumber must be at least 3 characters long");
    }
}