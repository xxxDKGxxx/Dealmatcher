namespace Dealmatcher.Backend.API.Endpoints.Example.Post;

public sealed class Post(IMediator mediator) : Endpoint<PostExampleRequest, ExampleDto>
{
    public override void Configure()
    {
        Version(1);
        Post("/example");
    }

    public override async Task HandleAsync(PostExampleRequest request, CancellationToken ct)
    {
        var command = new CreateExampleCommand(request.E);

        var result = await mediator.Send(command, ct);

        await result.SendResult(this, ct: ct);
    }
}
