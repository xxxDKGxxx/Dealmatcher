namespace Dealmatcher.Backend.API.Endpoints.Authentication.Register;

public sealed record CreateUserRequest(string Email, string Password, string Name, string Surname);
