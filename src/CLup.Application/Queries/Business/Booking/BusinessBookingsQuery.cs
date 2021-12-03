using System.Collections.Generic;
using CLup.Application.Shared;
using CLup.Application.Queries.Shared;
using MediatR;

namespace CLup.Application.Queries.Business.Booking
{
    public class BusinessBookingsQuery : IRequest<Result<List<BookingDto>>>
    {
        public string BusinessId { get; set; }
    }
}