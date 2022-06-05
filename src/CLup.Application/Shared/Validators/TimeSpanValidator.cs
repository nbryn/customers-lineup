using CLup.Domain.Shared.ValueObjects;
using FluentValidation;

namespace CLup.Application.Shared.Validators
{
    public class TimeSpanValidator : AbstractValidator<TimeSpan>
    {
        public TimeSpanValidator()
        {
            RuleFor(x => x.Start).NotEmpty();
            RuleFor(x => x.End).NotEmpty();
        }
    }
}