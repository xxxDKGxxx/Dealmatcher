using Dealmatcher.Backend.UseCases.Features.Bans.Create;

namespace Dealmatcher.Backend.API.Endpoints.Bans.Create;

public sealed class CreateBanEndpoint(
    IMediator mediator,
    IClaimsPrincipalManager claimsManager)
    : Endpoint<CreateBanRequest, BanDto>
{
    public override void Configure()
    {
        Version(1);
        Post("/bans");
        Roles("Admin");
        Description(d => d
            .Produces<BanDto>(201, "application/json")
            .Produces(400)
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .Produces(409)
            .Produces(500));
        Summary(s =>
        {
            s.Summary = "Ban a user";
            s.Description = "Creates a new ban for a user (admin only)";
            s.Response<BanDto>(201, "User banned successfully");
            s.Response(400, "Invalid ban data");
            s.Response(401, "Unauthorized");
            s.Response(403, "Forbidden - admin only");
            s.Response(404, "User not found");
            s.Response(409, "User already banned");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(CreateBanRequest request, CancellationToken ct)
    {
        var adminId = claimsManager.GetUserId(User);
        if (adminId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var command = new CreateBanCommand(adminId.Value, request.UserId, request.Reason, request.ExpiresAt);
        var result = await mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            await SendAsync(result.Value, 201, ct);
            return;
        }

        await result.SendResult(this, ct);
    }
}
