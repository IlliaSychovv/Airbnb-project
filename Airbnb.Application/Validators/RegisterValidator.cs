using Airbnb.Application.DTOs;
using FluentValidation;

namespace Airbnb.Application.Validators;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required!")
            .MinimumLength(2).WithMessage("Name must be at least 2 character long");
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required!")
            .EmailAddress().WithMessage("Email is invalid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required!")
            .MinimumLength(8).WithMessage("Password must be at least 8 character long")
            .Matches(@"[\!\@\#\$\%\^\&\*]").WithMessage("Password must contain at least one special character")
            .Matches(@"[A-Z]+").WithMessage("Password must contain at least one upper case letter");
    }
}