using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using Logic.Auth;
using Logic.DTO;
using Logic.Util;
using Data;

namespace Logic.Businesses
{
    [ApiController]
    [Authorize(Policy = Policies.User)]
    [Route("[controller]")]
    public class BusinessController : ControllerBase
    {
        private readonly IBusinessRepository _repository;
        private readonly IBusinessService _service;
        private readonly IDTOMapper _dtoMapper;

        public BusinessController(IBusinessRepository repository,
        IBusinessService service, IDTOMapper dtoMapper)
        {
            _repository = repository;
            _dtoMapper = dtoMapper;
            _service = service;

        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> NewBusiness([FromBody] CreateBusinessDTO dto)
        {
            string ownerEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            dto.OwnerEmail = ownerEmail;

            BusinessDTO business = await _service.RegisterBusiness(dto);

            return Ok(business);
        }

        [HttpGet]
        [Route("owner")]
        public async Task<IEnumerable<BusinessDTO>> FetchBusinessesForOwner()
        {
            string ownerEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var all = await _repository.FindBusinessesByOwner(ownerEmail);

            return all.Select(x => _dtoMapper.ConvertBusinessToDTO(x));
        }

        [HttpGet]
        [Route("all")]
        public async Task<IEnumerable<BusinessDTO>> FetchAll()
        {
            var all = await _repository.GetAll();

            return all.Select(x => _dtoMapper.ConvertBusinessToDTO(x));
        }

        [HttpGet]
        [Route("types")]
        public IEnumerable<string> FetchBusinessTypes()
        {
            return _dtoMapper.GetBusinessTypes();
        }
    }
}