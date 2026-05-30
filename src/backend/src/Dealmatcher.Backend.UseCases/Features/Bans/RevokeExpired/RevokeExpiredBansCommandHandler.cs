namespace Dealmatcher.Backend.UseCases.Features.Bans.RevokeExpired;

public sealed class RevokeExpiredBansCommandHandler(
    IRepository<User> userRepository) : ICommandHandler<RevokeExpiredBansCommand, Result>
{
    public async Task<Result> Handle(RevokeExpiredBansCommand request, CancellationToken cancellationToken)
    {
        var spec = new UsersWithExpiredBansSpec();
        var users = await userRepository.ListAsync(spec, cancellationToken);

        foreach (var user in users)
        {
            var expiredBans = user.Bans
                .Where(b => b.IsActive && b.ExpiresAt != null && b.ExpiresAt < DateTime.UtcNow)
                .ToList();

            foreach (var ban in expiredBans)
            {
                user.RevokeBan(ban.Id);
            }

            await userRepository.UpdateAsync(user, cancellationToken);
        }

        if (users.Count != 0)
        {
            await userRepository.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
