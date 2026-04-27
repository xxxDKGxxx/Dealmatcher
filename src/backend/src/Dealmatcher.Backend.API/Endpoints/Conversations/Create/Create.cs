using Dealmatcher.Backend.Domain.EntityAggregates.ConversationAggregate.Dto;

namespace Dealmatcher.Backend.API.Endpoints.Conversations.Create;

public sealed class Create(
    IMediator mediator,
    IClaimsPrincipalManager claimsManager)
    : Endpoint<CreateRequest, ConversationDto>
{
    public override void Configure()
    {
        Version(1);
        Post("/conversations");

        Description(d => d.Produces<ConversationDto>(201, "application/json")
                            .Produces(400)
                            .Produces(401)
                            .Produces(403)
                            .Produces(404)
                            .Produces(409)
                            .Produces(500)
        );

        Summary(s =>
        {
            s.Summary = "Create a new conversation";
            s.Description = "Creates a conversation between buyer and seller about an offer";
            s.Response<ConversationDto>(201, "Conversation created successfully");
            s.Response(400, "Invalid request");
            s.Response(401, "Unauthorized");
            s.Response(403, "Cannot create conversation with yourself");
            s.Response(404, "Offer not found");
            s.Response(409, "Conversation already exists");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(CreateRequest request, CancellationToken cancellationToken)
    {
        var userId = claimsManager.GetUserId(User);

        if (userId is null)
        {
            await SendUnauthorizedAsync(cancellation: cancellationToken);
            return;
        }

        var command = new CreateConversationCommand(userId.Value, request.OfferId, request.InitialMessage);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            await SendAsync(result.Value, statusCode: 201, cancellation: cancellationToken);
            return;
        }

        await result.SendResult(this, ct: cancellationToken);
    }
}
