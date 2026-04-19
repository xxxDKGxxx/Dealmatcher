namespace Dealmatcher.Backend.Infrastructure.Data.Config;

public sealed class OfferConfiguration : DealmatcherBaseEntityConfiguration<Offer>
{
    public override void Configure(EntityTypeBuilder<Offer> builder)
    {
        base.Configure(builder);

        builder.ToTable($"{nameof(Offer)}s");

        builder.HasQueryFilter(o => o.Seller.Status == UserStatus.Active);

        builder.Property(o => o.Title)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.OfferTitleMaxLength);

        builder.Property(o => o.Description)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.OfferDescriptionMaxLength);

        builder.Property(o => o.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(o => o.Availability)
            .IsRequired();

        builder.Property(o => o.Status)
            .HasConversion(
                s => s.Value,
                v => OfferStatus.FromValue(v))
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.StatusMaxLength);

        builder.HasOne(o => o.Seller)
            .WithMany()
            .HasForeignKey("SellerId")
            .OnDelete(DeleteBehavior.NoAction);

        builder.Navigation(o => o.Seller)
            .AutoInclude();

        builder.HasOne(o => o.Category)
            .WithMany()
            .HasForeignKey("CategoryId")
            .OnDelete(DeleteBehavior.NoAction);

        builder.Navigation(o => o.Category)
            .AutoInclude();

        builder.HasMany(o => o.Properties)
            .WithOne()
            .HasForeignKey("OfferId")
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Navigation(o => o.Properties)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .AutoInclude();
    }
}
