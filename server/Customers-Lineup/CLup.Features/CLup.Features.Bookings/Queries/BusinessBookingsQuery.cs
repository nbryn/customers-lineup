using System.Collections.Generic;

using MediatR;

using CLup.Features.Common;

namespace CLup.Features.Bookings.Queries
{
    public class BusinessBookingsQuery : IRequest<Result<List<BookingDTO>>>
    {
        public string BusinessId { get; set; }
    }
}