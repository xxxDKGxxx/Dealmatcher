namespace Dealmatcher.Backend.Infrastructure.Data.Config;

public sealed class ActivityConfiguration : DealmatcherBaseEntityConfiguration<Activity>
{
    public override void Configure(EntityTypeBuilder<Activity> builder)
    {
        base.Configure(builder);

        builder.ToTable("Activities");

        builder.HasOne(a => a.User)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(a => a.Offer)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(a => a.Action)
            .HasConversion(
                a => a.Value,
                v => ActivityAction.FromValue(v))
            .IsRequired();

        builder.Property(a => a.IPAddress)
            .HasConversion(
                ip => ip.ToString(),
                s => System.Net.IPAddress.Parse(s))
            .HasMaxLength(45)
            .IsRequired();

        builder.Property<Dictionary<string, string>>("_details")
            .HasColumnName("Details")
            .HasConversion(
                d => JsonSerializer.Serialize(d, JsonSerializerOptions.Default),
                s => JsonSerializer.Deserialize<Dictionary<string, string>>(s, JsonSerializerOptions.Default) ?? new())
            .HasColumnType("nvarchar(max)")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
