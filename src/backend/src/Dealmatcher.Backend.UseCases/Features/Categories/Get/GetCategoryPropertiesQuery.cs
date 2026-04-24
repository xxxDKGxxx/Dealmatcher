namespace Dealmatcher.Backend.UseCases.Features.Categories.Get;

public sealed record GetCategoryPropertiesQuery(string CategoryName)
  : IQuery<Result<List<PropertyDefinitionDto>>>;
