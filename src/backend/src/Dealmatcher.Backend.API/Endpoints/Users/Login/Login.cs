namespace Dealmatcher.Backend.API.Endpoints.Users.Login;

public sealed class Login(IMediator mediator) : Endpoint<LoginRequest, LoginDto>
{
    public override void Configure()
    {
        Version(1);
        Get("/users/login");
    }

    public override async Task HandleAsync(LoginRequest request, CancellationToken ct)
    {
        var command = new LoginCommand(request.Email, request.Password);

        var result = await mediator.Send(command, ct);

        await result.SendResult(this, ct: ct);
    }
}
