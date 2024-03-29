using CLup.Domain.Businesses.ValueObjects;
using CLup.Domain.Messages;
using CLup.Domain.Messages.Enums;
using CLup.Domain.Messages.ValueObjects;
using CLup.Domain.Users.ValueObjects;

namespace CLup.Infrastructure.Persistence.EntityConfigurations;

internal sealed class UserMessageConfiguration : IEntityTypeConfiguration<UserMessage>
{
    public void Configure(EntityTypeBuilder<UserMessage> builder)
    {
        builder.ToTable("User Messages");
        builder.HasKey(message => message.Id);
        builder.Property(message => message.Id)
            .ValueGeneratedNever()
            .HasConversion(
            messageId => messageId.Value,
            value => MessageId.Create(value));

        builder.Property(message => message.ReceiverId)
            .ValueGeneratedNever()
            .HasConversion(
                receiverId => receiverId.Value,
                value => BusinessId.Create(value));

        builder.Property(message => message.SenderId)
            .ValueGeneratedNever()
            .HasConversion(
                senderId => senderId.Value,
                value => UserId.Create(value));

        builder
            .Property(message => message.Type)
            .HasConversion(type => type.ToString("G"), type => Enum.Parse<MessageType>(type));

        builder.OwnsOne(message => message.MessageData, md =>
        {
            md.Property(messageData => messageData.Title)
                .HasColumnName("Title");

            md.Property(messageData => messageData.Content)
                .HasColumnName("Content");
        });

        builder.OwnsOne(message => message.Metadata, metaData =>
        {
            metaData.Property(messageMetadata => messageMetadata.DeletedBySender)
                .HasColumnName("DeletedBySender");

            metaData.Property(messageMetadata => messageMetadata.DeletedByReceiver)
                .HasColumnName("DeletedByReceiver");
        });
    }
}
