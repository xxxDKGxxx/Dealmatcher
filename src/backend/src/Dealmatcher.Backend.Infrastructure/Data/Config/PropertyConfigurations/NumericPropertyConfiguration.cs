namespace Dealmatcher.Backend.Infrastructure.Data.Config.PropertyConfigurations;

public sealed class NumericPropertyConfiguration : IEntityTypeConfiguration<NumericProperty>
{
    public void Configure(EntityTypeBuilder<NumericProperty> builder)
    {
        builder.Property(p => p.Value).HasColumnName("NumericValue");
    }
}
