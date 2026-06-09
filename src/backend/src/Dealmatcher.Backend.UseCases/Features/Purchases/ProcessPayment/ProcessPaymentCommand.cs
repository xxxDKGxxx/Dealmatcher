namespace Dealmatcher.Backend.UseCases.Features.Purchases.ProcessPayment;

public sealed record ProcessPaymentCommand(
    string SessionId,
    string RawBody) : ICommand<Result>;
