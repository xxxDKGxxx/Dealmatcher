namespace Dealmatcher.Backend.API.Endpoints.Cart.Update;

public sealed class UpdateItemRequestValidator : Validator<UpdateItemRequest>
{
    public UpdateItemRequestValidator()
    {
        RuleFor(r => r.CartItemId)
            .NotNull()
            .GreaterThanOrEqualTo(0);

        RuleFor(r => r.Quantity)
            .NotNull()
            .GreaterThanOrEqualTo(1);
    }
}
