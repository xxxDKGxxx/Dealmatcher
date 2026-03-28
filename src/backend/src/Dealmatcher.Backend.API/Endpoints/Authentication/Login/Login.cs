namespace Dealmatcher.Backend.API.Endpoints.Authentication.Login;

public sealed class Login(IMediator mediator) : Endpoint<LoginRequest, LoginDto>
{
    public override void Configure()
    {
        Version(1);
        Post("/users/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Email, request.Password);

        var result = await mediator.Send(command, cancellationToken);

        await result.SendResult(this, ct: cancellationToken);
    }
}
