using System.ComponentModel.DataAnnotations;

namespace Dealmatcher.Backend.Domain.EntityAggregates.UserAggregate;

public sealed class BasicUser(string email, string passwordHash, string name, string surname) : User(email, passwordHash, name, surname)
{
    public DateTime? BirthDate { get; private set; }
    public string? CompanyName { get; private set; }
    public string? Phone { get; private set; }
    public string? Address { get; private set; }

    public void UpdateBirthDate(DateTime? birthDate)
    {
        BirthDate = birthDate;
    }

    public void UpdateCompanyName(string? companyName)
    {
        if (!string.IsNullOrWhiteSpace(companyName))
        {
            CompanyName = companyName;
        }
    }

    public void UpdatePhone(string? phone)
    {
        if (!string.IsNullOrWhiteSpace(phone))
        {
            Phone = phone;
        }
    }

    public void UpdateAddress(string? address)
    {
        if (!string.IsNullOrWhiteSpace(address))
        {
            Address = address;
        }
    }
}
