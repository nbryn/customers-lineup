using System.Threading;
using System.Threading.Tasks;
using CLup.Application.Commands.User.Models;
using CLup.Application.Shared;
using CLup.Application.Shared.Extensions;
using CLup.Application.Shared.Interfaces;
using CLup.Domain.Booking;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CLup.Application.Commands.User.Handlers
{
    public class DeleteBookingHandler : IRequestHandler<DeleteBookingCommand, Result>
    {
        private readonly ICLupDbContext _context;

        public DeleteBookingHandler(ICLupDbContext context) => _context = context;

        public async Task<Result> Handle(DeleteBookingCommand command, CancellationToken cancellationToken)
        {
            return await _context.Bookings.Include(b => b.User).Include(b => b.Business).Include(b => b.TimeSlot).FirstOrDefaultAsync(x => x.Id == command.BookingId)
                     .FailureIf("Booking not found.")
                     .AddDomainEvent(booking => booking.DomainEvents.Add(new UserDeletedBookingEvent(booking)))
                     .Finally(booking => _context.RemoveAndSave(booking));
        }
    }
}