namespace Dealmatcher.Backend.API.Endpoints.Bans.Delete;

public sealed class DeleteBan(
    IMediator mediator,
    IClaimsPrincipalManager claimsManager)
    : Endpoint<DeleteBanRequest>
{
    public override void Configure()
    {
        Version(1);
        Delete("/bans/{BanId}");
        Roles("Admin");
        Description(d => d
            .Produces(204)
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .Produces(500));
        Summary(s =>
        {
            s.Summary = "Remove a ban";
            s.Description = "Removes a ban from a user (admin only)";
            s.Response(204, "Ban removed successfully");
            s.Response(401, "Unauthorized");
            s.Response(403, "Forbidden - admin only");
            s.Response(404, "Ban not found");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(DeleteBanRequest request, CancellationToken ct)
    {
        var adminId = claimsManager.GetUserId(User);
        if (adminId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var command = new DeleteBanCommand(adminId.Value, request.BanId);
        var result = await mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            await SendNoContentAsync(ct);
            return;
        }

        await result.SendResult(this, ct);
    }
}
