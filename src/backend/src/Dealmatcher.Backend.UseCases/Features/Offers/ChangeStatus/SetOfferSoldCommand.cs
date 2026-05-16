namespace Dealmatcher.Backend.UseCases.Features.Offers.ChangeStatus;

public sealed record SetOfferSoldCommand(int userId, int offerId) : ICommand<Result<OfferDto>>;
