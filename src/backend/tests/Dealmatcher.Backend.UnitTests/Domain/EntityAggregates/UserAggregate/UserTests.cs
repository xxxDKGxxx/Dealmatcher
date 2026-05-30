namespace Dealmatcher.Backend.UnitTests.Domain.EntityAggregates.UserAggregate;

public class UserTests
{
    private const string ValidEmail = "test@example.com";

    private static User CreateUser()
    {
        return new User(ValidEmail, "hashed_password", "Jan", "Kowalski");
    }

    [Fact]
    public void BanUser_ValidParameters_UpdatesStatusAndAddsBan()
    {
        var user = CreateUser();
        var expiresAt = DateTime.UtcNow.AddDays(7);

        user.BanUser("Złamanie regulaminu", 1, expiresAt);

        user.Status.ShouldBe(UserStatus.Banned);
        user.Bans.Count.ShouldBe(1);
        user.Bans.First().IsActive.ShouldBeTrue();
        user.Bans.First().ExpiresAt.ShouldBe(expiresAt);
    }

    [Fact]
    public void RevokeBan_LastActiveBan_ChangesStatusToActive()
    {
        var user = CreateUser();
        user.BanUser("Tymczasowy ban", 1, DateTime.UtcNow.AddDays(1));
        var banId = user.Bans.First().Id;

        user.RevokeBan(banId);

        user.Status.ShouldBe(UserStatus.Active);
        user.Bans.First().IsActive.ShouldBeFalse();
    }

    [Fact]
    public void RevokeBan_WithOtherActiveBans_KeepsStatusBanned()
    {
        var user = CreateUser();
        user.BanUser("Ban 1", 1, DateTime.UtcNow.AddDays(1));
        user.BanUser("Ban 2 (Permaban)", 1, null);

        var banToRevoke = user.Bans.First();

        user.RevokeBan(banToRevoke.Id);

        user.Status.ShouldBe(UserStatus.Banned);
        banToRevoke.IsActive.ShouldBeFalse();
        user.Bans.Last().IsActive.ShouldBeTrue();
    }
}
