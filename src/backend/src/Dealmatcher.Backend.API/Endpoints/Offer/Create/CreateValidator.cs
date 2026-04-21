namespace Dealmatcher.Backend.API.Endpoints.Offer.Create;

public sealed class CreateValidator : Validator<CreateRequest>
{
    public CreateValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(DataSchemaConstants.OfferTitleMaxLength);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(DataSchemaConstants.OfferDescriptionMaxLength);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.Availability).GreaterThan(0);
    }
}
