namespace Dealmatcher.Backend.Infrastructure.Data.Config;

public class ExampleConfiguration : DealmatcherBaseEntityConfiguration<Example>
{
    public override void Configure(EntityTypeBuilder<Example> builder)
    {
        base.Configure(builder);

        builder.ToTable($"{nameof(Example)}s");

        builder.Property(e => e.E)
            .IsRequired();
    }
}
