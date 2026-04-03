using Dealmatcher.Backend.Infrastructure.Data.Config;

namespace Dealmatcher.Backend.API.Endpoints.Authentication.Register;

public sealed class RegisterUserValidator : Validator<CreateUserRequest>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(DataSchemaConstants.EmailMaxLength)
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(DataSchemaConstants.NameMaxLength);

        RuleFor(x => x.Surname)
            .NotEmpty()
            .MaximumLength(DataSchemaConstants.SurnameMaxLength);
    }
}
