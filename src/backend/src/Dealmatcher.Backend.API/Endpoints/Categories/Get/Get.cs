namespace Dealmatcher.Backend.API.Endpoints.Categories.Get;

public class Get(IMediator mediator) : EndpointWithoutRequest<CategoryDto>
{
    public override void Configure()
    {
        Version(1);
        Get("/categories");
        AllowAnonymous();

        Description(d => d.Produces<CategoryDto>(200, "application/json").Produces(500));

        Summary(s =>
        {
            s.Summary = "Get all available categories";
            s.Description = "Returns a list of all product categories in the system";
            s.Response<CategoryDto>(200, "Categories retrieved successfully");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var request = new GetCategoriesQuery();
        var result = await mediator.Send(request, ct);

        await result.SendResult(this, ct);
    }
}
