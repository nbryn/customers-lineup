using CLup.Domain.Bookings;
using CLup.Domain.Businesses;
using CLup.Domain.Businesses.ValueObjects;
using CLup.Domain.Shared;
using CLup.Domain.Shared.ValueObjects;
using CLup.Domain.TimeSlots.ValueObjects;

namespace CLup.Domain.TimeSlots;

public sealed class TimeSlot : Entity, IHasDomainEvent
{
    private readonly List<Booking> _bookings = [];

    public TimeSlotId Id { get; private set; }

    public BusinessId BusinessId { get; private set; }

    public Business? Business { get; private set; }

    public string BusinessName { get; private set; }

    public int Capacity { get; private set; }

    public DateOnly Date { get; private set; }

    public TimeInterval TimeInterval { get; private set; }

    public List<DomainEvent> DomainEvents { get; set; } = [];

    public IReadOnlyList<Booking> Bookings => _bookings.AsReadOnly();

    public TimeSlot(
        BusinessId businessId,
        string businessName,
        int capacity,
        DateOnly date,
        TimeInterval timeInterval)
    {
        Guard.Against.Null(businessId);
        Guard.Against.NullOrWhiteSpace(businessName);
        Guard.Against.NegativeOrZero(capacity);
        Guard.Against.Null(date);
        Guard.Against.Null(timeInterval);

        BusinessId = businessId;
        BusinessName = businessName;
        Capacity = capacity;
        Date = date;
        TimeInterval = timeInterval;

        Id = TimeSlotId.Create(Guid.NewGuid());
    }

    private TimeSlot()
    {
    }

    public DomainResult IsAvailable()
    {
        if (Bookings.Count >= Capacity)
        {
            return DomainResult.Fail([TimeSlotErrors.NoCapacity]);
        }

        if (Date < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return DomainResult.Fail([TimeSlotErrors.InThePast]);
        }

        return DomainResult.Ok();
    }
}
