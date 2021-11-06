using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CLup.Application.Extensions;
using CLup.Application.Shared;
using CLup.Data;
using CLup.Domain.Businesses.TimeSlots;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CLup.Application.Businesses.TimeSlots.Commands
{
    public class GenerateTimeSlotsHandler : IRequestHandler<GenerateTimeSlotsCommand, Result>
    {
        private readonly CLupContext _context;
        private readonly IMapper _mapper;

        public GenerateTimeSlotsHandler(CLupContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result> Handle(GenerateTimeSlotsCommand command, CancellationToken cancellationToken)
        {
            return await _context.Businesses.FirstOrDefaultAsync(b => b.Id == command.BusinessId)
                    .FailureIf("Business not found.")
                    .AndThenDouble(() => _context.TimeSlots.FirstOrDefaultAsync(x => x.BusinessId == command.BusinessId && x.Start.Date == command.Start.Date))
                    .Ensure(timeSlot => timeSlot == null, "Time slots already generated for this date.")
                    .AndThen(business => TimeSlot.GenerateTimeSlots(business, command.Start))
                    .Finally(timeSlots => _context.AddAndSave(timeSlots.ToArray()));
        }
    }
}