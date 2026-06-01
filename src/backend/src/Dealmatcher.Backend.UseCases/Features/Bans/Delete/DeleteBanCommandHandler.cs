namespace Dealmatcher.Backend.UseCases.Features.Bans.Delete;

public sealed class DeleteBanCommandHandler(
    IRepository<User> userRepository) : ICommandHandler<DeleteBanCommand, Result>
{
    public async Task<Result> Handle(DeleteBanCommand request, CancellationToken cancellationToken)
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

        var spec = new UserByBanIdSpec(request.BanId);
        var targetUser = await userRepository.FirstOrDefaultAsync(spec, cancellationToken);

        if (targetUser is null)
        {
            return Result.NotFound($"Ban with id: {request.BanId} not found");
        }

        var ban = targetUser.Bans.FirstOrDefault(b => b.Id == request.BanId);
        if (ban is null || !ban.IsActive)
        {
            return Result.NotFound($"Active ban with id: {request.BanId} not found");
        }

        targetUser.RevokeBan(request.BanId);

        await userRepository.UpdateAsync(targetUser, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
