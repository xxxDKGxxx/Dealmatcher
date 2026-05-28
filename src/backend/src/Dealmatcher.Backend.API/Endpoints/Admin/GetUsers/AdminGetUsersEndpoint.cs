using Dealmatcher.Backend.API.Endpoints.Admin.GetOffers;

namespace Dealmatcher.Backend.API.Endpoints.Admin.GetUsers;

public sealed class AdminGetUsersEndpoint(
    IMediator mediator,
    IClaimsPrincipalManager claimsManager)
    : Endpoint<AdminGetUsersRequest, AdminUsersPageDto>
{
    public override void Configure()
    {
        Version(1);
        Get("/admin/users");
        Roles("Admin");
        Description(d => d
            .Produces<AdminUsersPageDto>(200, "application/json")
            .Produces(400)
            .Produces(401)
            .Produces(403)
            .Produces(500));
        Summary(s =>
        {
            s.Summary = "Get all users (admin)";
            s.Description = "Returns paginated list of all users in the system";
            s.Response<AdminUsersPageDto>(200, "Users retrieved successfully");
            s.Response(400, "Invalid request parameters");
            s.Response(401, "Unauthorized");
            s.Response(403, "Forbidden - admin only");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(AdminGetUsersRequest request, CancellationToken ct)
    {
        var userId = claimsManager.GetUserId(User);
        if (userId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var query = new AdminListUsersQuery(userId.Value, request.Page, request.Limit, request.Status);
        var result = await mediator.Send(query, ct);
        await result.SendResult(this, ct);
    }
}
