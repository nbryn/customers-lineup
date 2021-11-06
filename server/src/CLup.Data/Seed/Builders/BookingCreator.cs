using System;

using CLup.Domain.Bookings;

namespace CLup.Data.Seed.Builders
{
    public class BookingCreator
    {
        public static Booking Create(string userId, string businessId, string timeSlotId)
        {
            var booking = new Booking(userId, timeSlotId, businessId);
            booking.UpdatedAt = DateTime.Now;

            return booking;            
        }
    }
}