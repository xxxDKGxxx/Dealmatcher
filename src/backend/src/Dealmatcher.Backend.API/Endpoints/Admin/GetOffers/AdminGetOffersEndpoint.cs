namespace Dealmatcher.Backend.API.Endpoints.Admin.GetOffers;

public sealed class AdminGetOffersEndpoint(
    IMediator mediator,
    IClaimsPrincipalManager claimsManager)
    : Endpoint<AdminGetOffersRequest, AdminOffersPageDto>
{
    public override void Configure()
    {
        Version(1);
        Get("/admin/offers");
        Roles("Admin");
        Description(d => d
            .Produces<AdminOffersPageDto>(200, "application/json")
            .Produces(400)
            .Produces(401)
            .Produces(403)
            .Produces(500));
        Summary(s =>
        {
            s.Summary = "Get all offers (admin)";
            s.Description = "Returns paginated list of all offers in the system";
            s.Response<AdminOffersPageDto>(200, "Offers retrieved successfully");
            s.Response(400, "Invalid request parameters");
            s.Response(401, "Unauthorized");
            s.Response(403, "Forbidden - admin only");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(AdminGetOffersRequest request, CancellationToken ct)
    {
        var userId = claimsManager.GetUserId(User);
        if (userId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var query = new AdminListOffersQuery(userId.Value, request.Page, request.Limit, request.Status);
        var result = await mediator.Send(query, ct);
        await result.SendResult(this, ct);
    }
}
