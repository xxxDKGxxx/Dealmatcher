namespace Dealmatcher.Backend.UseCases.Features.Offers.Create;

public sealed class CreateOfferCommandHandler(
    IReadRepository<User> userRepository,
    IReadRepository<Category> categoryRepository,
    IRepository<Offer> offerRepository,
    IImageStorageService imageStorageService,
    IMapper mapper) : ICommandHandler<CreateOfferCommand, Result<OfferDto>>
{
    public async Task<Result<OfferDto>> Handle(CreateOfferCommand request, CancellationToken cancellationToken)
    {
        var activeUserByIdSpec = new ActiveUserByIdSpec(request.SellerId);
        var seller = await userRepository.FirstOrDefaultAsync(activeUserByIdSpec, cancellationToken);
        if (seller is null)
        {
            return Result.Invalid(new ValidationError($"Invalid seller Id: {request.SellerId}"));
        }

        var categoryByIdWithDefinitionsSpec = new CategoryWithDefinitionsByIdSpec(request.CategoryId);
        var category = await categoryRepository.FirstOrDefaultAsync(categoryByIdWithDefinitionsSpec, cancellationToken);
        if (category is null)
        {
            return Result.Invalid(new ValidationError($"Invalid category Id: {request.CategoryId}"));
        }

        var missingProperties = category.PropertyDefinitions
            .Where(pd => !request.Properties.ContainsKey(pd.Id.ToString()))
            .Select(pd => pd.Name)
            .ToList();

        if (missingProperties.Count > 0)
        {
            return Result.Invalid(new ValidationError($"Missing required properties for category '{category.Name}': {string.Join(", ", missingProperties)}"));
        }

        List<Property> properties = [];
        foreach (var propertyId in request.Properties.Keys)
        {
            if (!int.TryParse(propertyId, out int propertyIdParsed))
            {
                return Result.Invalid(new ValidationError($"Invalid property Id: {propertyId}"));
            }

            var propertyDefinition = category.PropertyDefinitions.Where(pd => pd.Id == propertyIdParsed).FirstOrDefault();
            if (propertyDefinition is null)
            {
                return Result.Invalid(new ValidationError($"Invalid property Id: {propertyId}"));
            }

            try
            {
                var property = propertyDefinition.CreatePropertyFromString(request.Properties[propertyId]);
                properties.Add(property);
            }
            catch
            {
                return Result.Invalid(new ValidationError($"Invalid property value: {request.Properties[propertyId]}"));
            }
        }

        var images = request.Images?.Take(10).ToList() ?? [];
        var uploadedImageUrls = new List<string>();
        if (images.Count > 0)
        {
            try
            {
                foreach (var image in images)
                {
                    var imageUrl = await imageStorageService.UploadImageAsync(
                        image.Content,
                        image.FileName,
                        image.ContentType,
                        cancellationToken);

                    uploadedImageUrls.Add(imageUrl);
                }
            }
            catch (Exception ex)
            {
                return Result.Error($"Error during image upload: {ex.Message}");
            }
        }

        Offer offer = new Offer(
            request.Title,
            request.Description,
            request.Price,
            uploadedImageUrls,
            seller,
            request.Tags,
            request.Availability,
            category,
            properties);

        await offerRepository.AddAsync(offer, cancellationToken);
        await offerRepository.SaveChangesAsync(cancellationToken);

        var offerDto = mapper.Map<OfferDto>(offer);
        return Result.Success(offerDto);
    }
}
