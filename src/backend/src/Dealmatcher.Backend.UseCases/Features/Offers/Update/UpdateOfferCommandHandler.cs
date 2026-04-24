namespace Dealmatcher.Backend.UseCases.Features.Offers.Update;

public sealed class UpdateOfferCommandHandler(
    IRepository<Offer> offerRepository,
    IReadRepository<Category> categoryRepository,
    IImageStorageService imageStorageService,
    IMapper mapper) : ICommandHandler<UpdateOfferCommand, Result<OfferDto>>
{
    public async Task<Result<OfferDto>> Handle(UpdateOfferCommand request, CancellationToken cancellationToken)
    {
        var offer = await offerRepository.GetByIdAsync(request.OfferId, cancellationToken);
        if (offer is null) return Result.NotFound();

        if (offer.Seller.Id != request.UserId)
        {
            return Result.Forbidden();
        }

        if (request.Title is not null)
        {
            offer.UpdateTitle(request.Title);
        }

        if (request.Description is not null)
        {
            offer.UpdateDescription(request.Description);
        }

        if (request.Price.HasValue)
        {
            offer.UpdatePrice(request.Price.Value);
        }

        if (request.Availability.HasValue)
        {
            offer.SetAvailability(request.Availability.Value);
        }

        if (request.Tags is not null)
        {
            offer.SetTags(request.Tags);
        }

        if (request.Images is not null)
        {
            var imagesToDelete = offer.Images.Except(request.Images).ToList();
            foreach (var imageUrl in imagesToDelete)
            {
                await imageStorageService.DeleteImageAsync(imageUrl, cancellationToken);
            }

            offer.SetImages(request.Images);
        }

        offer.SetStatusToDraft();

        if (request.Properties is not null)
        {
            await UpdatePropertiesAsync(offer, request.Properties, cancellationToken);
        }

        await offerRepository.SaveChangesAsync(cancellationToken);

        var offerDto = mapper.Map<OfferDto>(offer);
        return Result.Success(offerDto);
    }

    private async Task UpdatePropertiesAsync(Offer offer, Dictionary<string, string> newProperties, CancellationToken ct)
    {
        var categoryWithDefs = new CategoryWithDefinitionsByIdSpec(offer.Category.Id);
        var category = await categoryRepository.FirstOrDefaultAsync(categoryWithDefs, ct);

        if (category is null) return;

        List<Property> updatedProperties = [];
        foreach (var prop in newProperties)
        {
            if (int.TryParse(prop.Key, out int propId))
            {
                var def = category.PropertyDefinitions.FirstOrDefault(pd => pd.Id == propId);
                if (def is not null)
                {
                    updatedProperties.Add(def.CreatePropertyFromString(prop.Value));
                }
            }
        }

        offer.SetProperties(updatedProperties);
    }
}
