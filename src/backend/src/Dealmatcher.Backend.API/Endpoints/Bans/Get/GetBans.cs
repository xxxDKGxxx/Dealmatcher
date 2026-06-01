namespace Dealmatcher.Backend.API.Endpoints.Bans.Get;

public sealed class GetBansEndpoint(
    IMediator mediator,
    IClaimsPrincipalManager claimsManager)
    : Endpoint<GetBansRequest, List<BanDto>>
{
    public override void Configure()
    {
        Version(1);
        Get("/bans");
        Roles("Admin");
        Description(d => d
            .Produces<List<BanDto>>(200, "application/json")
            .Produces(401)
            .Produces(403)
            .Produces(500));
        Summary(s =>
        {
            s.Summary = "Get bans list";
            s.Description = "Returns list of bans (admin only)";
            s.Response<List<BanDto>>(200, "Bans retrieved successfully");
            s.Response(401, "Unauthorized");
            s.Response(403, "Forbidden - admin only");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(GetBansRequest request, CancellationToken ct)
    {
        var adminId = claimsManager.GetUserId(User);
        if (adminId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var query = new GetBansQuery(adminId.Value, request.UserId, request.Active);
        var result = await mediator.Send(query, ct);

        await result.SendResult(this, ct);
    }
}
