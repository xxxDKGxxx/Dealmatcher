namespace Dealmatcher.Backend.UseCases.Features.Authentication.Login;

public sealed record LoginDto(string AccessToken, UserDto User);
