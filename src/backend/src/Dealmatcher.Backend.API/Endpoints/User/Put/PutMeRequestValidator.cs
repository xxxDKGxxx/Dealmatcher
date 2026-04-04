namespace Dealmatcher.Backend.API.Endpoints.User.Put;

public class PutMeRequestValidator : Validator<PutMeRequest>
{
    public PutMeRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(DataSchemaConstants.NameMaxLength);

        RuleFor(x => x.Surname)
            .NotEmpty()
            .MaximumLength(DataSchemaConstants.SurnameMaxLength);
    }
}
