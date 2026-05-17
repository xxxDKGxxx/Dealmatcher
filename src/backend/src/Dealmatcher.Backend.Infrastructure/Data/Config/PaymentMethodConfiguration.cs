namespace Dealmatcher.Backend.Infrastructure.Data.Config;

public sealed class PaymentMethodConfiguration : DealmatcherBaseEntityConfiguration<PaymentMethod>
{
    public override void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        base.Configure(builder);

        builder.ToTable($"{nameof(PaymentMethod)}s");
        
        builder.Property(p => p.StringId)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.PaymentMethodIdMaxLength);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.PaymentMethodNameMaxLength);

        builder.Property(p => p.ProviderName)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.PaymentMethodProviderNameMaxLength);

        builder.Property(p => p.Icon)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.PaymentMethodIconMaxLength);
    }
}
