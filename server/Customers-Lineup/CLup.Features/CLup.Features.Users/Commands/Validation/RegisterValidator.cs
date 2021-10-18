using FluentValidation;

namespace CLup.Features.Users.Commands.Validation
{
    public class RegisterValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Zip).NotEmpty();
            RuleFor(x => x.Address).NotEmpty();
        }
    }
}