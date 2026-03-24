namespace Dealmatcher.Backend.Infrastructure.Data.Config;

public class UserConfiguration : DealmatcherBaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.ToTable($"{nameof(User)}s");

        builder.HasDiscriminator<string>("UserType")
            .HasValue<BasicUser>("BasicUser");

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

        builder.Property(u => u.IsPrivileged)
            .IsRequired();

        builder.Property(u => u.Status)
            .HasConversion(
                s => s.Value,
                v => UserStatus.FromValue(v))
            .IsRequired();
    }
}
