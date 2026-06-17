namespace Dealmatcher.Backend.UseCases.Features.Purchases.Initialize;

public sealed record InitializePurchaseCommand(
    int UserId,
    int OfferId,
    string DeliveryMethodId,
    string PaymentMethodId,
    int Quantity) : ICommand<Result<InitializePurchaseResult>>, ILoggableActivity<Result<InitializePurchaseResult>>
{
    public ActivityAction Action => ActivityAction.Purchase;

    public int? GetUserId(Result<InitializePurchaseResult> result) => UserId;

    public int? GetOfferId(Result<InitializePurchaseResult> result) => OfferId;

    public Dictionary<string, string> GetDetails(Result<InitializePurchaseResult> result) => new()
    {
        ["quantity"] = Quantity.ToString(),
        ["deliveryMethodId"] = DeliveryMethodId,
        ["paymentMethodId"] = PaymentMethodId,
    };
}
