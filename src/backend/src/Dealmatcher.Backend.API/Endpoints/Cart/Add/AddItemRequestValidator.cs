namespace Dealmatcher.Backend.API.Endpoints.Cart.Add;

public sealed class AddItemRequestValidator : Validator<AddItemRequest>
{
    public AddItemRequestValidator()
    {
        RuleFor(r => r.OfferId).NotNull().GreaterThanOrEqualTo(0);
        RuleFor(r => r.Quantity).NotNull().GreaterThanOrEqualTo(1);
    }
}
