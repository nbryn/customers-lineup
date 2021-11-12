using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CLup.Application.Queries.User.Models;
using CLup.Application.Shared;
using CLup.Application.Shared.Interfaces;
using CLup.Application.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CLup.Application.Queries.User.Handlers
{
    public class UsersNotEmployedByBusinessHandler : IRequestHandler<UsersNotEmployedByBusinessQuery, Result<UsersNotEmployedByBusinessResponse>>
    {
        private readonly IQueryDbContext _queryContext;
        private readonly IMapper _mapper;

        public UsersNotEmployedByBusinessHandler(IQueryDbContext queryContext, IMapper mapper)
        {
            _queryContext = queryContext;
            _mapper = mapper;
        }

        public async Task<Result<UsersNotEmployedByBusinessResponse>> Handle(UsersNotEmployedByBusinessQuery query, CancellationToken cancellationToken)
        {
            var notAlreadyEmployedByBusiness = new List<UserDto>();

            foreach (var user in await _queryContext.Users.ToListAsync())
            {
                var employee = await _queryContext.Employees.FirstOrDefaultAsync(e => e.UserId == user.Id &&
                                                                            e.BusinessId == query.BusinessId);
                if (employee == null)
                {
                    notAlreadyEmployedByBusiness.Add(_mapper.Map<UserDto>(user));
                }
            }

            var result = new UsersNotEmployedByBusinessResponse
            {
                BusinessId = query.BusinessId,
                Users = notAlreadyEmployedByBusiness,
            };

            return Result.Ok<UsersNotEmployedByBusinessResponse>(result);
        }
    }
}