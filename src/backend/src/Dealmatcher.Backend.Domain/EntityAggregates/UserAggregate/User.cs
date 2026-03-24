namespace Dealmatcher.Backend.Domain.EntityAggregates.UserAggregate;

public class User(string email, string passwordHash, string name, string surname) : DealmatcherEntityBase, IAggregateRoot
{
    public string Email { get; private set; } = email;
    public string PasswordHash { get; private set; } = passwordHash;
    public string Name { get; private set; } = name;
    public string Surname { get; private set; } = surname;
    public bool IsPrivileged { get; private set; } = false;
    public UserStatus Status { get; private set; } = UserStatus.Active;

    public void UpdateEmail(string email)
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            Email = email;
        }
    }

    public void UpdatePasswordHash(string passwordHash)
    {
        if (!string.IsNullOrWhiteSpace(passwordHash))
        {
            PasswordHash = passwordHash;
        }
    }

    public void UpdateName(string? name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }
    }


    public void UpdateSurname(string? surname)
    {
        if (!string.IsNullOrWhiteSpace(surname))
        {
            Surname = surname;
        }
    }

    public void GrantAdminPrivileges()
    {
        IsPrivileged = true;
    }

    public void RevokeAdminPrivileges()
    {
        IsPrivileged = false;
    }

    public void BanUser()
    {
        Status = UserStatus.Banned;
    }

    public void ActivateUserAccount()
    {
        Status = UserStatus.Active;
    }

    public void DeactivateUserAccount()
    {
        Status = UserStatus.Inactive;
    }

    public override void Delete()
    {
        DeactivateUserAccount();
    }
}
