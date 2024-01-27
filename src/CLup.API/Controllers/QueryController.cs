using AutoMapper;
using CLup.Application.Businesses;
using CLup.Application.Shared.Interfaces;
using CLup.Application.Shared.Util;
using CLup.Application.Users;
using CLup.Application.Users.Queries;
using CLup.Domain.Businesses.Enums;
using CLup.Domain.Businesses.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace CLup.API.Controllers;

[ApiController]
[Route("api/query")]
public class QueryController : AuthorizedControllerBase
{
    private readonly ICLupRepository _context;
    private readonly IMapper _mapper;

    public QueryController(
        ICLupRepository context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("user")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FetchUserAggregate()
    {
        var user = await _context.FetchUserAggregateById(GetUserIdFromJwt());
        if (user == null)
        {
            return NotFound("User was not found");
        }

        return Ok(_mapper.Map<UserDto>(user));
    }

    [HttpGet]
    [Route("business/all")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<BusinessDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FetchAllBusinesses()
    {
        var businesses = await _context.FetchAllBusinesses();

        return Ok(_mapper.Map<IList<BusinessDto>>(businesses));
    }

    [HttpGet]
    [Route("business/types")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult FetchBusinessTypes()
    {
        var types = EnumUtil
            .GetValues<BusinessType>()
            .Select(type => type.ToString("G"))
            .ToList();

        return Ok(types);
    }

    [Route("user/notEmployedByBusiness/{businessId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UsersNotEmployedByBusiness))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FetchAllUsersNotAlreadyEmployedByBusiness([FromRoute] Guid businessId)
    {
        var business = await _context.FetchBusinessAggregate(BusinessId.Create(businessId));
        if (business == null)
        {
            return NotFound();
        }

        var users = await _context.FetchUsersNotEmployedByBusiness(BusinessId.Create(businessId));

        return Ok(new UsersNotEmployedByBusiness()
            { BusinessId = businessId, Users = _mapper.Map<IList<UserDto>>(users) });
    }
}
