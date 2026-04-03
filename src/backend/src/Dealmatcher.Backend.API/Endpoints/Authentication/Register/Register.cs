namespace Dealmatcher.Backend.API.Endpoints.Authentication.Register;

public sealed class Register(IMediator mediator) : Endpoint<CreateUserRequest, UserDto>
{
    public override void Configure()
    {
        Version(1);
        Post("/users/register");
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Register a new user";
            s.Description = "Creates a new user account with INACTIVE status";
            s.Responses[201] = "User registered successfully";
            s.Responses[400] = "Invalid registration data";
            s.Responses[409] = "Email already exists";
            s.Responses[500] = "Internal server error";
        });
    }

    public override async Task HandleAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(
            request.Email,
            request.Password,
            request.Name,
            request.Surname);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            await SendAsync(result.Value, statusCode: 201, cancellation: cancellationToken);
        }
        else
        {
            await result.SendResult(this, ct: cancellationToken);
        }
    }
}
