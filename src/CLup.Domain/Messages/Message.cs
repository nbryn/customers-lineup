using CLup.Domain.Messages.Enums;
using CLup.Domain.Messages.ValueObjects;
using CLup.Domain.Shared;
using CLup.Domain.Shared.ValueObjects;

namespace CLup.Domain.Messages;

public abstract class Message : Entity
{
    public MessageId Id { get; }

    public virtual Id SenderId { get; }

    public virtual Id ReceiverId { get; }

    public MessageData MessageData { get; }

    public MessageType Type { get; }

    public MessageMetadata Metadata { get; private set; }

    protected Message(
        Id senderId,
        Id receiverId,
        MessageData messageData,
        MessageType type,
        MessageMetadata metadata)
    {
        Guard.Against.Null(senderId);
        Guard.Against.Null(receiverId);
        Guard.Against.Null(messageData);
        Guard.Against.EnumOutOfRange(type);
        Guard.Against.Null(metadata);

        SenderId = senderId;
        ReceiverId = receiverId;
        MessageData = messageData;
        Type = type;
        Metadata = metadata;
        CreatedAt = DateTime.Now;

        Id = MessageId.Create(Guid.NewGuid());
    }

    protected Message()
    {
    }

    public Message MarkAsDeleted(bool forReceiver)
    {
        Metadata = new MessageMetadata(!forReceiver || Metadata.DeletedBySender,
            forReceiver || Metadata.DeletedByReceiver);
        return this;
    }
}
