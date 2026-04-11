namespace Dealmatcher.Backend.Infrastructure.Data.Config.PropertyRelatedEnumConfigurations;

public sealed class PropertyRelatedEnumValueConfiguration : DealmatcherBaseEntityConfiguration<PropertyRelatedEnumValue>
{
    public override void Configure(EntityTypeBuilder<PropertyRelatedEnumValue> builder)
    {
        base.Configure(builder);

        builder.ToTable($"{nameof(PropertyRelatedEnumValue)}s");

        builder.Property(v => v.Name)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.PropertyRelatedEnumValueNameMaxLength);

        builder.Property(v => v.Value)
            .IsRequired();
    }
}
