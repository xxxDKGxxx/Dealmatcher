namespace Dealmatcher.Backend.API.Endpoints.Categories.Get;

public class GetProperties(IMediator mediator)
  : Endpoint<GetPropertiesRequest, List<PropertyDefinitionDto>>
{
  public override void Configure()
  {
    Version(1);
    Get("/categories/{CategoryName}/properties");
    AllowAnonymous();

    Description(d =>
      d.Produces<List<PropertyDefinitionDto>>(200, "application/json").Produces(404).Produces(500)
    );

    Summary(s =>
    {
      s.Summary = "Get properties for a specific category";
      s.Description = "Returns all configurable properties for the given category";
      s.Response<List<PropertyDefinitionDto>>(200, "Properties retrieved successfully");
      s.Response(404, "Category not found.");
      s.Response(500, "Internal server error");
    });
  }

  public override async Task HandleAsync(GetPropertiesRequest req, CancellationToken ct)
  {
    var request = new GetCategoryPropertiesQuery(req.CategoryName);
    var result = await mediator.Send(request, ct);

    await result.SendResult(this, ct);
  }
}
