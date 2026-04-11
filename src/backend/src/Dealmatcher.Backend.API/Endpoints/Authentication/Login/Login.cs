namespace Dealmatcher.Backend.API.Endpoints.Authentication.Login;

public sealed class Login(IMediator mediator) : Endpoint<LoginRequest, LoginDto>
{
    public override void Configure()
    {
        Version(1);
        Post("/users/login");
        AllowAnonymous();

        Description(d => d
            .Produces<LoginDto>(200, "application/json")
            .Produces(401)
            .Produces(403)
            .Produces(500));

        Summary(s =>
        {
            s.Summary = "Login user";
            s.Description = "Authenticates user with email and password";
            s.Response<LoginDto>(200, "Login successful");
            s.Response(401, "Invalid credentials");
            s.Response(403, "Account banned");
            s.Response(500, "Server error");
        });
    }

    public override async Task HandleAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Email, request.Password);

        var result = await mediator.Send(command, cancellationToken);

        await result.SendResult(this, ct: cancellationToken);
    }
}
