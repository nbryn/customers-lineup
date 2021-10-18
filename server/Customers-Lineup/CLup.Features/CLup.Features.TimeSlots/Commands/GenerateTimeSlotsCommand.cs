using System;

using MediatR;

using CLup.Features.Common;

namespace CLup.Features.TimeSlots.Commands
{
    public class GenerateTimeSlotsCommand : IRequest<Result>
    {
        public string BusinessId { get; set; }
        public DateTime Start { get; set; }
    }
}
