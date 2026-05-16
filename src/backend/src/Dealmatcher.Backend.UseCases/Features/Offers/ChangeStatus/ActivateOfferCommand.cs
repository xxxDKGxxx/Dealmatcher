namespace Dealmatcher.Backend.UseCases.Features.Offers.ChangeStatus;

public sealed record ActivateOfferCommand(
    int adminId,
    int offerId) : ICommand<Result<OfferDto>>;
