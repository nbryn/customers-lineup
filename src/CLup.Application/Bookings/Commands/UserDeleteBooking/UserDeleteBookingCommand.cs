using CLup.Application.Shared;
using MediatR;

namespace CLup.Application.Bookings.Commands.UserDeleteBooking
{
    using Shared.Result;

    public class UserDeleteBookingCommand : IRequest<Result>
    {
        public string UserEmail { get; set; }
        
        public string BookingId { get; set; }
    }
}