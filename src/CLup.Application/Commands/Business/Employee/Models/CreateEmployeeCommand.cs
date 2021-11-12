using CLup.Application.Shared;
using MediatR;

namespace CLup.Application.Commands.Business.Employee.Models
{
    public class CreateEmployeeCommand : IRequest<Result>
    {
        public string BusinessId { get; set; }
        public string UserId { get; set; }
        
        public string CompanyEmail { get; set; }
    }
}
