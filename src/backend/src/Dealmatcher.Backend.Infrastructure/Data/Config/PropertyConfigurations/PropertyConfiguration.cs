namespace Dealmatcher.Backend.Infrastructure.Data.Config.PropertyConfigurations;

public sealed class PropertyConfiguration : DealmatcherBaseEntityConfiguration<Property>
{
    public override void Configure(EntityTypeBuilder<Property> builder)
    {
        base.Configure(builder);

        builder.ToTable($"{nameof(Property)}s");

        builder.HasDiscriminator<string>("PropertyType")
            .HasValue<BooleanProperty>("Boolean")
            .HasValue<NumericProperty>("Numeric")
            .HasValue<SelectProperty>("Select")
            .HasValue<TextProperty>("Text");

        builder.HasOne(p => p.PropertyDefinition)
            .WithMany()
            .HasForeignKey("PropertyDefinitionId")
            .OnDelete(DeleteBehavior.NoAction);
    }
}
