using FluentValidation;

namespace CLup.Application.Bookings.Commands.Validation
{
    public class CreateBookingValidator : AbstractValidator<CreateBookingCommand>
    {
        public CreateBookingValidator()
        {
            RuleFor(b => b.UserId).NotNull();
            RuleFor(b => b.TimeSlotId).NotNull(); 
            RuleFor(b => b.TimeSlotId).NotNull();           
        }
    }
}