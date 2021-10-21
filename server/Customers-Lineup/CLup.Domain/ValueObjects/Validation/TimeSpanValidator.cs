using FluentValidation;

namespace CLup.Domain.ValueObjects.Validation
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