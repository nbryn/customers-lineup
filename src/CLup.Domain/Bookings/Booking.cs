using System.Collections.Generic;
using CLup.Domain.Businesses;
using CLup.Domain.Businesses.TimeSlots;
using CLup.Domain.Shared;
using CLup.Domain.Users;

namespace CLup.Domain.Bookings
{
    public class Booking : Entity, IHasDomainEvent
    {
        public string UserId { get; private set; }

        public User User { get; private set; }

        public string TimeSlotId { get; private set; }

        public TimeSlot TimeSlot { get; private set; }

        public string BusinessId { get; private set; }

        public Business Business { get; private set; }

        public List<DomainEvent> DomainEvents { get; set; } = new();

        public Booking(string userId, string timeSlotId, string businessId)
        {
            UserId = userId;
            TimeSlotId = timeSlotId;
            BusinessId = businessId;
        }
    }
}