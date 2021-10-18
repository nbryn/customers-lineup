using MediatR;

using CLup.Features.Common;

namespace CLup.Features.Employees.Commands
{
    public class CreateEmployeeCommand : IRequest<Result>
    {

        public string BusinessId { get; set; }
        public string UserId { get; set; }
        public string CompanyEmail { get; set; }
    }
}
