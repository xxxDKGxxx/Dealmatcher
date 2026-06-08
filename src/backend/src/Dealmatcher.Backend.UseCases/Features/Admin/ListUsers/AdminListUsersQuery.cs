namespace Dealmatcher.Backend.UseCases.Features.Admin.ListUsers;

public sealed record AdminListUsersQuery(
    int AdminId,
    int Page,
    int Limit,
    string Status) : IQuery<Result<AdminUsersPageDto>>;
