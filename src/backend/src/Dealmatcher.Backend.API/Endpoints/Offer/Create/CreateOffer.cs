namespace Dealmatcher.Backend.API.Endpoints.Offer.Create;

public sealed class CreateOffer(
    IMediator mediator,
    IClaimsPrincipalManager claimsManager)
    : Endpoint<CreateOfferRequest, OfferDto>
{
    public override void Configure()
    {
        Version(1);
        Post("/offers");
        AllowFileUploads();

        Description(d => d.Produces<OfferDto>(201, "application/json").Produces(400).Produces(401).Produces(500));

        Summary(s =>
        {
            s.Summary = "Create a new offer";
            s.Description = "Creates a new offer in DRAFT status pending validation";
            s.Response<OfferDto>(201, "Offer created successfully");
            s.Response(400, "Invalid offer data");
            s.Response(401, "Unauthorized - user not logged in");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(CreateOfferRequest request, CancellationToken cancellationToken)
    {
        var userId = claimsManager.GetUserId(User);
        if (userId is null)
        {
            await SendUnauthorizedAsync(cancellation: cancellationToken);
            return;
        }

        var fileDtos = request.Images?
            .Take(10)
            .Select(f => new FileDto(f.OpenReadStream(), f.FileName, f.ContentType))
            .ToList() ?? [];

        var command = new CreateOfferCommand(
            request.Title,
            request.Description,
            request.Price,
            fileDtos,
            userId.Value,
            request.CategoryId,
            request.Tags,
            request.Properties,
            request.Availability);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            await SendAsync(result.Value, statusCode: 201, cancellation: cancellationToken);
        }
        else
        {
            await result.SendResult(this, ct: cancellationToken);
        }
    }
}
