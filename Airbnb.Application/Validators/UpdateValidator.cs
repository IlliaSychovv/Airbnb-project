using Airbnb.Application.DTO.Authorization;
using FluentValidation;

namespace Airbnb.Application.Validators;

public class UpdateValidator : AbstractValidator<UpdateDto>
{
    public UpdateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty");
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required!")
            .EmailAddress().WithMessage("Email is invalid");
        
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+\d{10,15}$")
            .WithMessage("The phone number must start with '+' and contain 10 to 15 digits");
    }
}