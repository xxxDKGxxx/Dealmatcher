using Dealmatcher.Backend.Domain.EntityAggregates.ConversationAggregate.Messages;

namespace Dealmatcher.Backend.Infrastructure.Data.Config;

public sealed class MessageConfiguration : DealmatcherBaseEntityConfiguration<Message>
{
    public override void Configure(EntityTypeBuilder<Message> builder)
    {
        base.Configure(builder);

        builder.ToTable("Messages");

        builder.HasOne(m => m.Sender).WithMany().IsRequired().OnDelete(DeleteBehavior.ClientCascade);

        builder
          .Property(m => m.Content)
          .HasMaxLength(DataSchemaConstants.MessageMaxLength)
          .IsRequired();

        builder
          .Property(m => m.Status)
          .HasConversion(ms => ms.Value, v => MessageStatus.FromValue(v))
          .IsRequired();

        builder.Navigation(m => m.Sender).AutoInclude();
    }
}
