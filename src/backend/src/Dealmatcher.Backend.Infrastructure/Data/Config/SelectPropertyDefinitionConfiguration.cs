using System.Text.Json;

namespace Dealmatcher.Backend.Infrastructure.Data.Config;

public sealed class SelectPropertyDefinitionConfiguration : IEntityTypeConfiguration<SelectPropertyDefinition>
{
    public void Configure(EntityTypeBuilder<SelectPropertyDefinition> builder)
    {
        builder.Property<List<string>>("_values")
            .HasColumnName("Values")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new())
            .HasColumnType("nvarchar(max)");
    }
}
