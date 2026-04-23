namespace Dealmatcher.Backend.UseCases.Features.Categories.Get;

public class GetCategoryPropertiesQueryHandler(
  IReadRepository<Category> categoriesRepository,
  IMapper mapper
) : IQueryHandler<GetCategoryPropertiesQuery, Result<List<PropertyDefinitionDto>>>
{
    public async Task<Result<List<PropertyDefinitionDto>>> Handle(
      GetCategoryPropertiesQuery request,
      CancellationToken cancellationToken
    )
    {
        var spec = new CategoryByNameSpec(request.CategoryName);
        var category = await categoriesRepository.SingleOrDefaultAsync(spec, cancellationToken);

        if (category is null)
        {
            return Result.NotFound();
        }

        return Result.Success(
          category.PropertyDefinitions.Select(mapper.Map<PropertyDefinitionDto>).ToList()
        );
    }
}
