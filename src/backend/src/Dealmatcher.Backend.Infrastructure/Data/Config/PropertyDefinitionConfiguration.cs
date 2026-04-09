namespace Dealmatcher.Backend.Infrastructure.Data.Config;

public sealed class PropertyDefinitionConfiguration : DealmatcherBaseEntityConfiguration<PropertyDefinition>
{
    public override void Configure(EntityTypeBuilder<PropertyDefinition> builder)
    {
        base.Configure(builder);

        builder.ToTable($"{nameof(PropertyDefinition)}s");

        builder.HasDiscriminator<string>("DefinitionType")
            .HasValue<BooleanPropertyDefinition>("Boolean")
            .HasValue<NumericPropertyDefinition>("Numeric")
            .HasValue<SelectPropertyDefinition>("Select")
            .HasValue<TextPropertyDefinition>("Text");

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.PropertyDefinitionNameMaxLength);

        builder.Property(p => p.Type)
            .IsRequired();
    }
}
