namespace Dealmatcher.Backend.Infrastructure.Data.Config;

public class BasicUserConfiguration : IEntityTypeConfiguration<BasicUser>
{
    public void Configure(EntityTypeBuilder<BasicUser> builder)
    {
        builder.Property(u => u.BirthDate)
            .IsRequired(false);

        builder.Property(u => u.CompanyName)
            .IsRequired(false)
            .HasMaxLength(DataSchemaConstants.CompanyNameMaxLength);

        builder.Property(u => u.Phone)
            .IsRequired(false)
            .HasMaxLength(DataSchemaConstants.PhoneMaxLength);

        builder.Property(u => u.Address)
            .IsRequired(false)
            .HasMaxLength(DataSchemaConstants.AddressMaxLength);
    }
}
