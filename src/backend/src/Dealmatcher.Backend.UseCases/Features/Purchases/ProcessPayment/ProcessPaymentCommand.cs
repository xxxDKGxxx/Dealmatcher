namespace Dealmatcher.Backend.UseCases.Features.Purchases.ProcessPayment;

public sealed record ProcessPaymentCommand(
    string SessionId,
    string ProviderStatus) : ICommand<Result>;
