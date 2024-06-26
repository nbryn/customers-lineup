using CLup.Domain.Bookings;
using CLup.Domain.Shared.ValueObjects;

namespace CLup.Application.Bookings;

public sealed class BookingDto
{
    public Guid Id { get; init; }

    public Guid TimeSlotId { get; init; }

    public Guid UserId { get; init; }

    public Guid BusinessId { get; init; }

    public string Business { get; init; }

    public Coords Coords { get; init; }

    public string Street { get; init; }

    public string UserEmail { get; init; }

    public TimeInterval Interval { get; init; }

    public string Date { get; init; }

    public string Capacity { get; init; }

    public static BookingDto FromBooking(Booking booking)
    {
        return new BookingDto()
        {
            Id = booking.Id.Value,
            UserId = booking.UserId.Value,
            BusinessId = booking.BusinessId.Value,
            TimeSlotId = booking.TimeSlotId.Value,
            Business = $"{booking.Business.BusinessData.Name} - {booking.Business.Address.City}",
            Date = booking.TimeSlot.Date.ToString("dd/MM/yyyy"),
            Interval = booking.TimeSlot.TimeInterval,
            Capacity = $"{booking.TimeSlot.Bookings.Count}/{booking.TimeSlot.Capacity}",
            Coords = booking.Business.Address.Coords,
            Street = booking.Business.Address.Street,
            UserEmail = booking.User.UserData.Email,
        };
    }
}
