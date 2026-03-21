namespace Dealmatcher.Backend.Infrastructure.Data.Config;

public class UserConfiguration : DealmatcherBaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.ToTable($"{nameof(User)}s");

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.EmailMaxLength);

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.NameMaxLength);

        builder.Property(u => u.Surname)
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.SurnameMaxLength);

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

        builder.Property(u => u.IsPrivileged)
            .IsRequired();

        builder.Property(u => u.Status)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(DataSchemaConstants.StatusMaxLength);

    }
}
