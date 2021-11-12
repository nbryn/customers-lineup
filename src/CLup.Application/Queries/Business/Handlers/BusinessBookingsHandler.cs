using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CLup.Application.Queries.Business.Models;
using CLup.Application.Shared;
using CLup.Application.Shared.Extensions;
using CLup.Application.Shared.Interfaces;
using CLup.Application.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CLup.Application.Queries.Business.Handlers
{

    public class BusinessBookingsHandler : IRequestHandler<BusinessBookingsQuery, Result<List<BookingDto>>>
    {
        private readonly IQueryDbContext _queryContext;
        private readonly IMapper _mapper;

        public BusinessBookingsHandler(IQueryDbContext queryContext, IMapper mapper)
        {
            _mapper = mapper;
            _queryContext = queryContext;
        }

        public async Task<Result<List<BookingDto>>> Handle(BusinessBookingsQuery query, CancellationToken cancellationToken)
        {
            return await _queryContext.Businesses.FirstOrDefaultAsync(b => b.Id == query.BusinessId)
                    .ToResult()
                    .EnsureDiscard(business => business != null)
                    .Finally(async () =>
                    {
                        var bookings = await _queryContext.Bookings
                                        .Include(x => x.TimeSlot)
                                        .ThenInclude(x => x.Business)
                                        .Include(x => x.User)
                                        .Where(x => x.BusinessId == query.BusinessId)
                                        .OrderBy(x => x.TimeSlot.Start).ToListAsync();

                        return bookings.Select(_mapper.Map<BookingDto>).ToList();
                    });
        }
    }
}