namespace Dealmatcher.Backend.Infrastructure.Data.Config;

public sealed class CategoryConfiguration : DealmatcherBaseEntityConfiguration<Category>
{
    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        base.Configure(builder);

        builder.ToTable("Categories");

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.CategoryNameMaxLength);

        builder.HasMany(c => c.PropertyDefinitions)
            .WithOne()
            .HasForeignKey("CategoryId")
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Navigation(c => c.PropertyDefinitions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
