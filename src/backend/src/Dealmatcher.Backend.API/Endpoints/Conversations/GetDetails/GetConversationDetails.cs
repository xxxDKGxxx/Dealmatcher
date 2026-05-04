using Dealmatcher.Backend.UseCases.Features.Conversations.GetDetails;

namespace Dealmatcher.Backend.API.Endpoints.Conversations.GetDetails;

public class GetConversationDetails(
    IMediator mediator,
    IClaimsPrincipalManager claimsManager)
    : Endpoint<GetConversationDetailsRequest, ConversationDetailDto>
{
    public override void Configure()
    {
        Version(1);
        Get("/conversations/{ConversationId}");

        Description(d => d.Produces<ConversationDetailDto>(200, "application/json")
                            .Produces(401)
                            .Produces(403)
                            .Produces(404)
                            .Produces(500)
        );

        Summary(s =>
        {
            s.Summary = "Get conversation details";
            s.Description = "Returns conversation with all messages";
            s.Response<ConversationDetailDto>(200, "Conversation details retrieved successfully");
            s.Response(401, "Unauthorized");
            s.Response(403, "Forbidden - not your conversation");
            s.Response(404, "Conversation not found");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(GetConversationDetailsRequest request, CancellationToken cancellationToken)
    {
        var userId = claimsManager.GetUserId(User);

        if (userId is null)
        {
            await SendUnauthorizedAsync(cancellation: cancellationToken);
            return;
        }

        var query = new GetConversationDetailsQuery(request.ConversationId, userId.Value);

        var result = await mediator.Send(query, cancellationToken);

        if (result.IsSuccess)
        {
            await SendAsync(result.Value, statusCode: 200, cancellation: cancellationToken);
            return;
        }

        await result.SendResult(this, ct: cancellationToken);
    }
}
