namespace Dealmatcher.Backend.API.Endpoints.Conversations.Get;

public class GetConversations(
    IMediator mediator,
    IClaimsPrincipalManager claimsPrincipalManager) :
    EndpointWithoutRequest<List<ConversationDto>>
{
    public override void Configure()
    {
        Version(1);
        Get("/conversations");

        Description(d => d
            .Produces<List<ConversationDto>>(200, "application/json")
            .Produces(401)
            .Produces(500));

        Summary(s =>
        {
            s.Summary = "Get user conversations";
            s.Description = "Returns all conversations for the authenticated user";
            s.Response<List<ConversationDto>>(200, "Conversations retrieved successfully");
            s.Response(401, "Unauthorized");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = claimsPrincipalManager.GetUserId(User);

        if (userId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var query = new GetUserConversationsQuery(userId.Value);
        var result = await mediator.Send(query, ct);

        await result.SendResult(this, ct);
    }
}
