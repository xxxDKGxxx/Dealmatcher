namespace Dealmatcher.Backend.Infrastructure.Data.Config;

public sealed class SelectPropertyDefinitionConfiguration : IEntityTypeConfiguration<SelectPropertyDefinition>
{
    public void Configure(EntityTypeBuilder<SelectPropertyDefinition> builder)
    {
        builder.HasOne(s => s.PropertyRelatedEnum)
            .WithMany()
            .HasForeignKey("PropertyRelatedEnumId")
            .OnDelete(DeleteBehavior.NoAction);
    }
}
