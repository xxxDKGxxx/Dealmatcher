namespace Dealmatcher.Backend.Domain.EntityAggregates.ActivityAggregate;
public abstract class ActivityAction(string name, int value) : SmartEnum<ActivityAction>(name, value)
{
    public static readonly ActivityAction Create = new CreateAction();
    public static readonly ActivityAction Update = new UpdateAction();
    public static readonly ActivityAction Delete = new DeleteAction();
    public static readonly ActivityAction View = new ViewAction();
    public static readonly ActivityAction Purchase = new PurchaseAction();
    public static readonly ActivityAction StatusChange = new StatusChangeAction();
    public static readonly ActivityAction Login = new LoginAction();
    public static readonly ActivityAction Logout = new LogoutAction();
    private sealed class CreateAction() : ActivityAction("CREATE", 0);
    private sealed class UpdateAction() : ActivityAction("UPDATE", 1);
    private sealed class DeleteAction() : ActivityAction("DELETE", 2);
    private sealed class ViewAction() : ActivityAction("VIEW", 3);
    private sealed class PurchaseAction() : ActivityAction("PURCHASE", 4);
    private sealed class StatusChangeAction() : ActivityAction("STATUS_CHANGE", 5);
    private sealed class LoginAction() : ActivityAction("LOGIN", 6);
    private sealed class LogoutAction() : ActivityAction("LOGOUT", 7);
}
