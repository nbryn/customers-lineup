using CLup.Domain.Users;
using FluentValidation;

namespace CLup.Application.Shared.Validators
{
    public class UserDataValidator : AbstractValidator<UserData>
    {
        public UserDataValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(4);
        }
    }
}