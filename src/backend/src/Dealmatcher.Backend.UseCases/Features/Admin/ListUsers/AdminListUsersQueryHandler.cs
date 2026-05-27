namespace Dealmatcher.Backend.UseCases.Features.Admin.ListUsers;

public sealed class AdminListUsersQueryHandler(
    IReadRepository<User> usersRepository,
    IMapper mapper) : IQueryHandler<AdminListUsersQuery, Result<AdminUsersPageDto>>
{
    public async Task<Result<AdminUsersPageDto>> Handle(AdminListUsersQuery request, CancellationToken cancellationToken)
    {
        var admin = await usersRepository.GetByIdAsync(request.AdminId, cancellationToken);

        if (admin is null)
        {
            return Result.Unauthorized($"Admin with id: {request.AdminId} doesn't exist");
        }

        if (!admin.IsPrivileged)
        {
            return Result.Forbidden($"User id: {request.AdminId} isn't privileged");
        }

        if (request.Limit <= 0)
        {
            return Result.Invalid(new ValidationError($"Invalid Limit value: {request.Limit}"));
        }

        if (request.Page <= 0)
        {
            return Result.Invalid(new ValidationError($"Invalid Page value: {request.Page}"));
        }

        UserStatus status;
        try
        {
            status = UserStatus.FromName(request.Status, true);
        }
        catch
        {
            return Result.Invalid(new ValidationError($"User status: {request.Status} is invalid"));
        }

        var pagedUsersByStatusSpec = new PagedUsersByStatusSpec(request.Page, request.Limit, status);
        var users = await usersRepository.ListAsync(pagedUsersByStatusSpec, cancellationToken);

        var usersByStatusSpec = new UsersByStatusSpec(status);
        var total = await usersRepository.CountAsync(usersByStatusSpec, cancellationToken);
        var totalPages = (int)Math.Ceiling(total / (double)request.Limit);
        var dtos = users.Select(mapper.Map<UserDto>).ToList();

        return Result.Success(new AdminUsersPageDto(dtos, total, Math.Min(request.Page, totalPages), totalPages));
    }
}
