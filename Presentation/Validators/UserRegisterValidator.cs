namespace Presentation.Validators;

using FluentValidation;
using Presentation.Dtos.Auth;

public class UserRegisterValidator : AbstractValidator<UserRegisterRequest>
{
    public UserRegisterValidator()
    {
        RuleFor(usr => usr.Email)
            .NotEmpty().NotNull()
            .EmailAddress().WithMessage("Not email provided");

        RuleFor(usr => usr.Password)
            .MinimumLength(6).WithMessage("The password should be at least 6 characters");
    }
}