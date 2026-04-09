namespace Dealmatcher.Backend.Infrastructure.Data.Config.PropertyRelatedEnumConfigurations;

public sealed class PropertyRelatedEnumConfiguration : DealmatcherBaseEntityConfiguration<PropertyRelatedEnum>
{
    public override void Configure(EntityTypeBuilder<PropertyRelatedEnum> builder)
    {
        base.Configure(builder);

        builder.ToTable($"{nameof(PropertyRelatedEnum)}s");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.PropertyRelatedEnumNameMaxLength);

        builder.HasMany(e => e.Values)
            .WithOne(v => v.PropertyRelatedEnum)
            .HasForeignKey("PropertyRelatedEnumId")
            .OnDelete(DeleteBehavior.NoAction);

        builder.Navigation(e => e.Values)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
