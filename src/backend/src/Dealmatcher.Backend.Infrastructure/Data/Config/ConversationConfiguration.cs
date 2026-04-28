using Dealmatcher.Backend.Domain.EntityAggregates.ConversationAggregate;

namespace Dealmatcher.Backend.Infrastructure.Data.Config;

public sealed class ConversationConfiguration : DealmatcherBaseEntityConfiguration<Conversation>
{
  public override void Configure(EntityTypeBuilder<Conversation> builder)
  {
    base.Configure(builder);

    builder.ToTable("Conversations");

    // Client Cascade won't actually delete related entities since Offer and User do not have Conversations loaded in memory, but will disable constraint violations
    builder.HasOne(c => c.Offer).WithMany().OnDelete(DeleteBehavior.ClientCascade).IsRequired();
    builder.HasOne(c => c.Buyer).WithMany().OnDelete(DeleteBehavior.ClientCascade).IsRequired();

    builder.HasMany(c => c.Messages).WithOne().OnDelete(DeleteBehavior.ClientCascade).IsRequired();

    builder
      .Property(c => c.Status)
      .IsRequired()
      .HasConversion(cs => cs.Value, c => ConversationStatus.FromValue(c));

    builder.Navigation(c => c.Messages).HasField("_messages").AutoInclude();
    builder.Navigation(c => c.Offer).AutoInclude();
    builder.Navigation(c => c.Buyer).AutoInclude();
  }
}
