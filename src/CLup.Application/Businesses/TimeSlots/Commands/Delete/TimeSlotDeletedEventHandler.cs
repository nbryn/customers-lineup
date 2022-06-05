using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CLup.Application.Shared.Interfaces;
using CLup.Application.Shared.Models;
using CLup.Domain.Businesses.TimeSlots;
using MediatR;

namespace CLup.Application.Businesses.TimeSlots.Commands.Delete
{
    public class TimeSlotDeletedEventHandler : INotificationHandler<DomainEventNotification<TimeSlotDeletedEvent>>
    {
        private readonly ICLupRepository _repository;

        public TimeSlotDeletedEventHandler(ICLupRepository repository) => _repository = repository;

        public async Task Handle(
            DomainEventNotification<TimeSlotDeletedEvent> @event,
            CancellationToken cancellationToken)
        {
            var domainEvent = @event.DomainEvent;
            var users = domainEvent.TimeSlot.Bookings.Select(booking => booking.User);
            foreach (var user in users)
            {
                domainEvent.BusinessOwner.BusinessDeletedBookingMessage(domainEvent.TimeSlot.Business, user.Id);
            }

            await _repository.UpdateEntity(domainEvent.TimeSlot.Business.Id, domainEvent.TimeSlot.Business);
        }
    }
}