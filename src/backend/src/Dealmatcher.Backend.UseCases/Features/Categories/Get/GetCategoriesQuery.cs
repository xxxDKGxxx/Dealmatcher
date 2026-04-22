namespace Dealmatcher.Backend.UseCases.Features.Categories.Get;

public sealed record GetCategoriesQuery() : IQuery<Result<List<CategoryDto>>>;
