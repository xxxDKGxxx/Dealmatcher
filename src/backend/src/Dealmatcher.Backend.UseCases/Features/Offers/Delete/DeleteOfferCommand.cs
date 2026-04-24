namespace Dealmatcher.Backend.UseCases.Features.Offers.Delete;

public sealed record DeleteOfferCommand(int OfferId, int UserId) : ICommand<Result>;
