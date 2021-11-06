using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CLup.Application.Extensions;
using CLup.Application.Shared;
using CLup.Data;
using CLup.Domain.Bookings;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace CLup.Application.Bookings.Commands
{
    public class CreateBookingHandler : IRequestHandler<CreateBookingCommand, Result>
    {
        private readonly IValidator<Booking> _validator;
        private readonly CLupContext _context;
        private readonly IMapper _mapper;

        public CreateBookingHandler(
            IValidator<Booking> validator,
            CLupContext context, 
            IMapper mapper) 
        {
            _validator = validator;
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result> Handle(CreateBookingCommand command, CancellationToken cancellationToken)
        {
            return await _context.Bookings.FirstOrDefaultAsync(x =>
                                                           x.TimeSlotId == command.TimeSlotId &&
                                                           x.UserId == command.UserId)
                    .ToResult()
                    .EnsureDiscard(booking => booking == null, "You already have a booking for this time slot.")
                    .FailureIf(() => _context.TimeSlots
                                        .Include(x => x.Bookings)
                                        .FirstOrDefaultAsync(x => x.Id == command.TimeSlotId), "Time Slot does not exists.")

                    .EnsureDiscard(timeSlot => timeSlot.Bookings.Count() < timeSlot.Capacity, "This time slot is full.")
                    .AndThen(() => _mapper.Map<Booking>(command))
                    .Validate(_validator)
                    .Finally(booking => _context.AddAndSave(booking));
        }
    }
}