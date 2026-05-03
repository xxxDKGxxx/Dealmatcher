namespace Dealmatcher.Backend.API.Endpoints.Conversations.SendMessage;

public class SendMessage(IMediator mediator, IClaimsPrincipalManager claimsPrincipalManager)
  : Endpoint<SendMessageRequest, MessageDto>
{
    public override void Configure()
    {
        Post("/conversations/{ConversationId}/messages");
        Version(1);

        Description(b =>
          b.Produces<MessageDto>(201, "application/json")
            .Produces(400)
            .Produces(401)
            .Produces(403)
            .Produces(404)
            .Produces(409)
            .Produces(500)
        );

        Summary(s =>
        {
            s.Summary = "Send a message";
            s.Description = "Sends a new message in the conversation";
            s.Response<MessageDto>(201, "Message sent successfully");
            s.Response(400, "Invalid message content");
            s.Response(401, "Unauthorized");
            s.Response(403, "Forbidden - not your conversation");
            s.Response(404, "Conversation not found");
            s.Response(409, "Conversation is closed");
            s.Response(500, "Internal server error");
        });
    }

    public override async Task HandleAsync(SendMessageRequest req, CancellationToken ct)
    {
        var userId = claimsPrincipalManager.GetUserId(User);
        if (userId is null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var command = new SendMessageCommand(req.ConversationId, userId.Value, req.Content);

        var result = await mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            await SendAsync(result.Value, 201, ct);
            return;
        }

        await result.SendResult(this, ct);
    }
}
