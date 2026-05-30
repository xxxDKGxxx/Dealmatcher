namespace Dealmatcher.Backend.Infrastructure.Data.Config;

public class BanConfiguration : DealmatcherBaseEntityConfiguration<Ban>
{
    public override void Configure(EntityTypeBuilder<Ban> builder)
    {
        base.Configure(builder);

        builder.ToTable("Bans");

        builder.Property(b => b.Reason)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.BanReasonMaxLength);

        builder.Property(b => b.IssuedAt)
            .IsRequired();

        builder.Property(b => b.IsActive)
            .IsRequired();
    }
}
