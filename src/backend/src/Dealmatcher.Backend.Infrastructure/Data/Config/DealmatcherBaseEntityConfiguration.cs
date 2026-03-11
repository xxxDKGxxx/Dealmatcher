
namespace Dealmatcher.Backend.Infrastructure.Data.Config;

public class DealmatcherBaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : DealmatcherEntityBase
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.DeletedAt).
            IsRequired(false);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false)
            .IsRequired();
    }
}
