namespace Dealmatcher.Backend.API.Endpoints.Authentication.Register;

public sealed class RegisterUserValidator : Validator<CreateUserRequest>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Surname)
            .NotEmpty();
    }
}
