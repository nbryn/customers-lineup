using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CLup.Application.Shared;
using CLup.Application.Shared.Extensions;
using CLup.Application.Shared.Interfaces;
using CLup.Domain.Businesses.ValueObjects;
using CLup.Domain.Messages;
using CLup.Domain.Messages.Enums;
using CLup.Domain.Users.ValueObjects;
using FluentValidation;
using MediatR;

namespace CLup.Application.Messages.Commands.SendMessage;

public sealed class SendMessageHandler : IRequestHandler<SendMessageCommand, Result>
{
    private readonly IValidator<Message> _validator;
    private readonly ICLupRepository _repository;
    private readonly IMapper _mapper;

    public SendMessageHandler(
        IValidator<Message> validator,
        ICLupRepository repository,
        IMapper mapper)
    {
        _validator = validator;
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result> Handle(SendMessageCommand command, CancellationToken cancellationToken)
        => await _repository.FetchUserAggregate(UserId.Create(command.SenderId))
            .ToResult()
            .AndThenAsync(async user =>
            {
                var business = await _repository.FetchBusinessAggregate(BusinessId.Create(command.SenderId));
                return new { business, user };
            })
            .Ensure(entry => entry.user != null || entry.business != null, HttpCode.BadRequest,
                MessageErrors.InvalidSender)
            .AndThenAsync(async _ =>
            {
                var user = await _repository.FetchUserAggregate(UserId.Create(command.ReceiverId));
                var business = await _repository.FetchBusinessAggregate(BusinessId.Create(command.ReceiverId));
                return new { business, user };
            })
            .Ensure(entry => entry.user != null || entry.business != null, HttpCode.BadRequest,
                MessageErrors.InvalidReceiver)
            .AndThen(_ => _mapper.Map<Message>(command))
            .Validate(_validator)
            .FinallyAsync(message => _repository.AddAndSave(message));
}