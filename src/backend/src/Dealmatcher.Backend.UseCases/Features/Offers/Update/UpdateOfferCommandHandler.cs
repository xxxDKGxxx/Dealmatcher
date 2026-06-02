namespace Dealmatcher.Backend.UseCases.Features.Offers.Update;

public sealed class UpdateOfferCommandHandler(
    IRepository<Offer> offerRepository,
    IReadRepository<Category> categoryRepository,
    IReadRepository<Purchase> purchaseRepository,
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

        var pendingPurchasesSpec = new PendingPurchasesByOfferIdSpec(request.OfferId);
        var pendingPurchases = await purchaseRepository.ListAsync(pendingPurchasesSpec, cancellationToken);
        if (pendingPurchases.Count > 0)
        {
            return Result.Conflict("Cannot edit offer with active pending purchases");
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
            var propertiesResult = await UpdatePropertiesAsync(offer, request.Properties, cancellationToken);

            if (!propertiesResult.IsSuccess)
            {
                return Result.Invalid(propertiesResult.ValidationErrors);
            }
        }

        await offerRepository.SaveChangesAsync(cancellationToken);

        var offerDto = mapper.Map<OfferDto>(offer);
        return Result.Success(offerDto);
    }

    private async Task<Result> UpdatePropertiesAsync(Offer offer, Dictionary<string, string> newProperties, CancellationToken ct)
    {
        var categoryWithDefs = new CategoryWithDefinitionsByIdSpec(offer.Category.Id);
        var category = await categoryRepository.FirstOrDefaultAsync(categoryWithDefs, ct);

        if (category is null)
        {
            return Result.Invalid(new ValidationError($"Category with Id {offer.Category.Id} not found."));
        }

        var missingProperties = category.PropertyDefinitions
            .Where(pd => !newProperties.ContainsKey(pd.Id.ToString()))
            .Select(pd => pd.Name)
            .ToList();

        if (missingProperties.Count > 0)
        {
            return Result.Invalid(new ValidationError($"Missing required properties for category '{category.Name}': {string.Join(", ", missingProperties)}"));
        }

        List<Property> updatedProperties = [];
        foreach (var propertyId in newProperties.Keys)
        {
            if (!int.TryParse(propertyId, out int propertyIdParsed))
            {
                return Result.Invalid(new ValidationError($"Invalid property Id: {propertyId}"));
            }

            var propertyDefinition = category.PropertyDefinitions.FirstOrDefault(pd => pd.Id == propertyIdParsed);

            if (propertyDefinition is null)
            {
                return Result.Invalid(new ValidationError($"Invalid property Id: {propertyId} for category {category.Name}"));
            }

            try
            {
                var existingProperty = offer.Properties.FirstOrDefault(p => p.PropertyDefinition.Id == propertyIdParsed);
                if (existingProperty is not null)
                {
                    propertyDefinition.UpdateProperty(existingProperty, newProperties[propertyId]);
                    updatedProperties.Add(existingProperty);
                }
                else
                {
                    var property = propertyDefinition.CreatePropertyFromString(newProperties[propertyId]);
                    updatedProperties.Add(property);
                }
            }
            catch
            {
                return Result.Invalid(new ValidationError($"Invalid property value: {newProperties[propertyId]} for {propertyDefinition.Name}"));
            }
        }

        offer.SetProperties(updatedProperties);
        return Result.Success();
    }
}
