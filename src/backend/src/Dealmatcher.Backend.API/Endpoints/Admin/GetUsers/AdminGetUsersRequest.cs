namespace Dealmatcher.Backend.API.Endpoints.Admin.GetUsers;

public sealed record AdminGetUsersRequest
{
    public int Page { get; init; } = 1;
    public int Limit { get; init; } = 20;
    public string Status { get; init; } = "ACTIVE";
}
