namespace Dealmatcher.Backend.Infrastructure.Data.Config;

public sealed class DeliveryMethodConfiguration : DealmatcherBaseEntityConfiguration<DeliveryMethod>
{
    public override void Configure(EntityTypeBuilder<DeliveryMethod> builder)
    {
        base.Configure(builder);

        builder.ToTable("DeliveryMethods");

        builder.Property(d => d.StringId)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.DeliveryMethodIdMaxLength);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.DeliveryMethodNameMaxLength);

        builder.Property(d => d.Description)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.DeliveryMethodDescriptionMaxLength);

        builder.Property(d => d.ProviderName)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.DeliveryMethodProviderNameMaxLength);

        builder.Property(d => d.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(d => d.EstimatedDays)
            .IsRequired();
    }
}
