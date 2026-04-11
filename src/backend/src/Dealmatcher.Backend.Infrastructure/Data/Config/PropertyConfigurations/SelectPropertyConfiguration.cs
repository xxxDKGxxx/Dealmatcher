namespace Dealmatcher.Backend.Infrastructure.Data.Config.PropertyConfigurations;

public sealed class SelectPropertyConfiguration : IEntityTypeConfiguration<SelectProperty>
{
    public void Configure(EntityTypeBuilder<SelectProperty> builder)
    {
        builder.Property(p => p.Value).HasColumnName("SelectValue");
    }
}
