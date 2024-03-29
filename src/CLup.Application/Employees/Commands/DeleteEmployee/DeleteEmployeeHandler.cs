using CLup.Application.Shared;
using CLup.Application.Shared.Extensions;
using CLup.Application.Shared.Interfaces;
using CLup.Domain.Businesses;
using CLup.Domain.Employees;

namespace CLup.Application.Employees.Commands.DeleteEmployee;

public sealed class DeleteEmployeeHandler : IRequestHandler<DeleteEmployeeCommand, Result>
{
    private readonly ICLupRepository _repository;

    public DeleteEmployeeHandler(ICLupRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(DeleteEmployeeCommand command, CancellationToken cancellationToken)
        => await _repository.FetchBusinessAggregate(command.OwnerId, command.BusinessId)
            .FailureIfNotFound(BusinessErrors.NotFound)
            .FailureIfNotFound(business => business?.GetEmployeeById(command.EmployeeId), EmployeeErrors.NotFound)
            .AndThen(employee => employee?.Business.RemoveEmployee(employee))
            .FinallyAsync(_ => _repository.SaveChangesAsync(true, cancellationToken));
}
