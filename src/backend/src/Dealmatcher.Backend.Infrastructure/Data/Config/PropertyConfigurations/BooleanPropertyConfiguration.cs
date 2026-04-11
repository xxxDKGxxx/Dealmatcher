namespace Dealmatcher.Backend.Infrastructure.Data.Config.PropertyConfigurations;

public sealed class BooleanPropertyConfiguration : IEntityTypeConfiguration<BooleanProperty>
{
    public void Configure(EntityTypeBuilder<BooleanProperty> builder)
    {
        builder.Property(p => p.Value).HasColumnName("BooleanValue");
    }
}
