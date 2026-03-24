namespace Dealmatcher.Backend.Infrastructure.Data.Config;

public class DealmatcherBaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : DealmatcherEntityBase
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired(false);
    }
}
