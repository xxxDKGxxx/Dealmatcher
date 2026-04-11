namespace Dealmatcher.Backend.UseCases.Features.Offers.Create;

public sealed class CreateOfferCommandHandler(
    IReadRepository<User> userRepository,
    IReadRepository<Category> categoryRepository,
    IRepository<Offer> offerRepository,
    IMapper mapper) : ICommandHandler<CreateOfferCommand, Result<OfferDto>>
{
    public async Task<Result<OfferDto>> Handle(CreateOfferCommand request, CancellationToken cancellationToken)
    {
        var activeUserByIdSpec = new ActiveUserByIdSpec(request.SellerId);
        var seller = await userRepository.FirstOrDefaultAsync(activeUserByIdSpec, cancellationToken);
        if (seller is null)
        {
            return Result.Invalid();
        }

        var categoryByIdWithDefinitionsSpec = new CategoryWithDefinitionsByIdSpec(request.CategoryId);
        var category = await categoryRepository.FirstOrDefaultAsync(categoryByIdWithDefinitionsSpec, cancellationToken);
        if (category is null)
        {
            return Result.Invalid();
        }

        List<Property> properties = [];
        foreach (var propertyName in request.Properties.Keys)
        {
            var propertyDefinition = category.PropertyDefinitions.Where(pd => pd.Name == propertyName).FirstOrDefault();
            if (propertyDefinition is null)
            {
                return Result.Invalid();
            }

            try
            {
                var property = propertyDefinition.CreatePropertyString(request.Properties[propertyName]);
                properties.Add(property);
            }
            catch
            {
                return Result.Invalid();
            }
        }

        Offer offer = new Offer(
            request.Title,
            request.Description,
            request.Price,
            [], // TODO: handling zdjec
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
