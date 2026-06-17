namespace Dealmatcher.Backend.UseCases.Features.Bans.Create;

public sealed class CreateBanCommandHandler(
    IRepository<User> userRepository,
    IMapper mapper) : ICommandHandler<CreateBanCommand, Result<BanDto>>
{
    public async Task<Result<BanDto>> Handle(CreateBanCommand request, CancellationToken cancellationToken)
    {
        var admin = await userRepository.GetByIdAsync(request.AdminId, cancellationToken);
        if (admin is null)
        {
            return Result.Unauthorized();
        }

        if (!admin.IsPrivileged)
        {
            return Result.Forbidden();
        }

        var targetUser = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (targetUser is null)
        {
            return Result.NotFound($"User with id: {request.UserId} not found");
        }

        if (targetUser.Status == UserStatus.Banned)
        {
            return Result.Conflict("User is already banned");
        }

        targetUser.BanUser(request.Reason, admin, request.ExpiresAt);

        await userRepository.UpdateAsync(targetUser, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        var newBan = targetUser.Bans.Last();
        var banDto = mapper.Map<BanDto>(newBan);

        return Result.Success(banDto);
    }
}
