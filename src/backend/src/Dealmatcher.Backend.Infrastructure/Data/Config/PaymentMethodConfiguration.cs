namespace Dealmatcher.Backend.Infrastructure.Data.Config;

public sealed class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.Property(p => p.StringId)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.PaymentMethodIdMaxLength);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength (DataSchemaConstants.PaymentMethodNameMaxLength);

        builder.Property(p => p.ProviderName)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.PaymentMethodProviderNameMaxLength);

        builder.Property(p => p.Icon)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.PaymentMethodIconMaxLength);
    }
}
