namespace Dealmatcher.Backend.Infrastructure.Data.Config;

public sealed class PurchaseConfiguration : DealmatcherBaseEntityConfiguration<Purchase>
{
    public override void Configure(EntityTypeBuilder<Purchase> builder)
    {
        base.Configure(builder);

        builder.ToTable($"{nameof(Purchase)}s");

        builder.Property(p => p.Quantity).IsRequired();
        builder.Property(p => p.TotalPrice).HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.DeliveryProviderId).IsRequired().HasMaxLength(50);
        builder.Property(p => p.PaymentProviderId).IsRequired().HasMaxLength(50);
        builder.Property(p => p.PaymentSessionId).HasMaxLength(200);

        builder.Property(p => p.Status)
            .HasConversion(s => s.Value, v => PurchaseStatus.FromValue(v))
            .IsRequired();

        builder.HasOne(p => p.Buyer)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(p => p.Offer)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Navigation(p => p.Offer).AutoInclude();
        builder.Navigation(p => p.Buyer).AutoInclude();
    }
}
