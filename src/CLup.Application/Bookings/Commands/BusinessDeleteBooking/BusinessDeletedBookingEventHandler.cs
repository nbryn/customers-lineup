using CLup.Application.Shared.Interfaces;
using CLup.Application.Shared.Models;
using CLup.Domain.Bookings.Events;

namespace CLup.Application.Bookings.Commands.BusinessDeleteBooking;

public sealed class BusinessDeletedBookingEventHandler : INotificationHandler<DomainEventNotification<BusinessDeletedBookingEvent>>
{
    private readonly ICLupRepository _repository;

    public BusinessDeletedBookingEventHandler(ICLupRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(DomainEventNotification<BusinessDeletedBookingEvent> @event, CancellationToken cancellationToken)
    {
        var domainEvent = @event.DomainEvent;
        domainEvent.BookingOwner.BookingDeletedMessage(domainEvent.Booking.UserId);

        await _repository.SaveChangesAsync(true, cancellationToken);
    }
}
