using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using CLup.Domain.Messages;
using CLup.Domain.Users;

namespace CLup.Data.EntityConfigurations
{
    class UserMessageEntityTypeConfiguration : IEntityTypeConfiguration<UserMessage>
    {
        public void Configure(EntityTypeBuilder<UserMessage> userMessageConfiguration)
        {
            userMessageConfiguration.ToTable("userMessages");
            userMessageConfiguration.HasKey(b => b.Id);

            userMessageConfiguration.OwnsOne(m => m.MessageData, md =>
            {
                md.Property(md => md.Title)
                    .HasColumnName("Title");
                    
                md.Property(md => md.Content)
                    .HasColumnName("Content");
            });

            userMessageConfiguration
                    .Property(b => b.Type)
                    .HasConversion(b => b.ToString("G"), b => Enum.Parse<MessageType>(b));

            userMessageConfiguration
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages);

            userMessageConfiguration
                .HasOne(m => m.Receiver)
                .WithMany(b => b.ReceivedMessages);
        }
    }
}