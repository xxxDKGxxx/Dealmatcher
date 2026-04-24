using Dealmatcher.Backend.UseCases.Features.Offers.Search;

namespace Dealmatcher.Backend.API.Endpoints.Offer.Search;

public sealed class Search(IMediator mediator) : Endpoint<SearchRequest, List<OfferDto>>
{
    public override void Configure()
    {
        Version(1);
        AllowAnonymous();
        Post("offers/search");

        Description(d => d.Produces<List<OfferDto>>(200)
                            .Produces(204)
                            .Produces(400)
                            .Produces(500)
        );

        Summary(s =>
        {
            s.Summary = "Search for offers";
            s.Description = "Creates a search query and returns matching offers";
            s.Response<List<OfferDto>>(200, "Search results retrieved successfully");
            s.Response(204, "No offers found matching the criteria");
            s.Response(400, "Invalid search parameters");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(SearchRequest request, CancellationToken cancellationToken)
    {
        var query = new SearchOffersQuery(
            request.CategoryId,
            request.MinPrice,
            request.MaxPrice,
            request.Tags,
            request.Properties,
            request.SearchPhrase,
            request.Limit);

        var result = await mediator.Send(query, cancellationToken);

        await result.SendResult(this, cancellationToken);
    }
}
