namespace Dealmatcher.Backend.UseCases.Features.Categories.Get;

public sealed class GetCategoriesQueryHandler(
  IReadRepository<Category> categoriesRepository,
  IMapper mapper
) : IQueryHandler<GetCategoriesQuery, Result<List<CategoryDto>>>
{
    public async Task<Result<List<CategoryDto>>> Handle(
      GetCategoriesQuery request,
      CancellationToken cancellationToken
    )
    {
        var categories = await categoriesRepository.ListAsync(cancellationToken);

        return Result.Success(categories.Select(mapper.Map<CategoryDto>).ToList());
    }
}
