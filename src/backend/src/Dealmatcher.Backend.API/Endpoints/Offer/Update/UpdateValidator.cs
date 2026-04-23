namespace Dealmatcher.Backend.API.Endpoints.Offer.Update;

public sealed class UpdateValidator : Validator<UpdateRequest>
{
    public UpdateValidator()
    {
        RuleFor(x => x.OfferId)
            .GreaterThan(0);

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(DataSchemaConstants.OfferTitleMaxLength)
            .When(x => x.Title != null);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(DataSchemaConstants.OfferDescriptionMaxLength)
            .When(x => x.Description != null);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Price != null);

        RuleFor(x => x.Availability)
            .GreaterThan(0)
            .When(x => x.Availability != null);
    }
}
