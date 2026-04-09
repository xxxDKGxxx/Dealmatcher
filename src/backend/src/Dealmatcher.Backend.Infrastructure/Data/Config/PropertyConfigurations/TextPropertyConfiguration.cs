namespace Dealmatcher.Backend.Infrastructure.Data.Config.PropertyConfigurations;

public sealed class TextPropertyConfiguration : IEntityTypeConfiguration<TextProperty>
{
    public void Configure(EntityTypeBuilder<TextProperty> builder)
    {
        builder.Property(p => p.Value)
            .HasColumnName("TextValue")
            .HasMaxLength(DataSchemaConstants.TextValueMaxLength);
    }
}
