using CLup.Application.Bookings;
using CLup.Application.Employees;
using CLup.Application.Messages;
using CLup.Application.TimeSlots;
using CLup.Domain.Businesses;
using CLup.Domain.Businesses.Enums;
using CLup.Domain.Shared.ValueObjects;

namespace CLup.Application.Businesses;

public sealed class BusinessAggregateDto
{
    public Guid Id { get; init; }

    public Guid OwnerId { get; init; }

    public string Name { get; init; }

    public Address Address { get; init; }

    public TimeInterval BusinessHours { get; init; }

    public int TimeSlotLengthInMinutes { get; init; }

    public BusinessType Type { get; init; }

    public int Capacity { get; init; }

    public required IList<BookingDto> Bookings { get; init; }

    public required IList<EmployeeDto> Employees { get; init; }

    public required IList<MessageDto> ReceivedMessages { get; init; }

    public required IList<MessageDto> SentMessages { get; init; }

    public required IList<TimeSlotDto> TimeSlots { get; init; }

    public static BusinessAggregateDto FromBusiness(Business business)
    {
        return new BusinessAggregateDto()
        {
            Id = business.Id.Value,
            OwnerId = business.OwnerId.Value,
            Name = business.BusinessData.Name,
            Address = business.Address,
            BusinessHours = business.BusinessHours,
            Capacity = business.BusinessData.Capacity,
            TimeSlotLengthInMinutes = business.BusinessData.TimeSlotLengthInMinutes,
            Type = business.Type,
            Bookings = business.Bookings.Select(BookingDto.FromBooking).ToList(),
            Employees = business.Employees.Select(EmployeeDto.FromEmployee).ToList(),
            TimeSlots = business.TimeSlots.Select(TimeSlotDto.FromTimeSlot).ToList(),
            ReceivedMessages = business.ReceivedMessages
                    .Where(message => !message.Metadata.DeletedByReceiver)
                    .Select(MessageDto.FromMessage)
                    .ToList(),
            SentMessages = business.SentMessages
                    .Where(message => !message.Metadata.DeletedBySender)
                    .Select(MessageDto.FromMessage)
                    .ToList(),
        };
    }
}
