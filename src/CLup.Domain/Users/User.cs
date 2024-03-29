using CLup.Domain.Bookings;
using CLup.Domain.Bookings.ValueObjects;
using CLup.Domain.Businesses;
using CLup.Domain.Messages;
using CLup.Domain.Messages.Enums;
using CLup.Domain.Messages.ValueObjects;
using CLup.Domain.Shared;
using CLup.Domain.Shared.ValueObjects;
using CLup.Domain.TimeSlots.ValueObjects;
using CLup.Domain.Users.Enums;
using CLup.Domain.Users.ValueObjects;

namespace CLup.Domain.Users;

public sealed class User : Entity, IAggregateRoot
{
    private readonly List<BusinessMessage> _receivedMessages = new();
    private readonly List<UserMessage> _sentMessages = new();
    private readonly List<Business> _businesses = new();
    private readonly List<Booking> _bookings = new();

    public UserId Id { get; }

    public UserData UserData { get; private set; }

    public Address Address { get; private set; }

    public Role Role { get; private set; }

    public IReadOnlyList<BusinessMessage> ReceivedMessages => _receivedMessages.AsReadOnly();

    public IReadOnlyList<UserMessage> SentMessages => _sentMessages.AsReadOnly();

    public IReadOnlyList<Business> Businesses => _businesses.AsReadOnly();

    public IReadOnlyList<Booking> Bookings => _bookings.AsReadOnly();

    public bool IsBusinessOwner => Businesses.Count > 0;

    public User(UserData userData, Address address, Role role)
    {
        Guard.Against.Null(userData);
        Guard.Against.Null(address);
        Guard.Against.EnumOutOfRange(role);

        UserData = userData;
        Address = address;
        Role = role;

        Id = UserId.Create(Guid.NewGuid());
    }

    private User()
    {
    }

    public User UpdateRole(Role role)
    {
        Role = role;
        return this;
    }

    public Booking? GetBookingById(BookingId bookingId) =>
        _bookings.Find(booking => booking.Id.Value == bookingId.Value);

    public bool BookingExists(TimeSlotId timeSlotId) =>
        _bookings.Exists(booking => booking.TimeSlot.Id.Value == timeSlotId.Value);

    public Message? GetMessageById(MessageId messageId, bool receivedMessage) =>
        receivedMessage
            ? GetReceivedMessageById(messageId)
            : GetSendMessageById(messageId);

    public UserMessage? GetSendMessageById(MessageId id) =>
        _sentMessages.Find(message => message.Id.Value == id.Value);

    public BusinessMessage? GetReceivedMessageById(MessageId id) =>
        _receivedMessages.Find(message => message.Id.Value == id.Value);

    public Booking RemoveBooking(Booking booking)
    {
        _bookings.Remove(booking);
        return booking;
    }

    public DomainResult CreateBooking(Booking booking)
    {
        if (BookingExists(booking.TimeSlotId))
        {
            return DomainResult.Fail(new List<Error>() { UserErrors.BookingExists });
        }

        _bookings.Add(booking);
        return DomainResult.Ok();
    }

    public User Update(Address address, string name, string email)
    {
        Address = address;
        UserData = new UserData(name, email, UserData.Password);

        return this;
    }

    public void BookingDeletedMessage(Booking booking)
    {
        var content =
            $"The user with email {UserData.Email} deleted her/his booking at {booking.TimeSlot.Date:dd/MM/yyyy}.";
        var messageData = new MessageData($"Booking Deleted - {booking.Business.BusinessData.Name}", content);
        var metadata = new MessageMetadata(false, false);
        var message = new UserMessage(Id, booking.Business.Id, messageData, MessageType.BookingDeleted, metadata);

        _sentMessages.Add(message);
    }
}
