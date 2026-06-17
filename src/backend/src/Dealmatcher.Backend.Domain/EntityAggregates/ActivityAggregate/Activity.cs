using System.Collections.ObjectModel;
using System.Net;

namespace Dealmatcher.Backend.Domain.EntityAggregates.ActivityAggregate;

public sealed class Activity : DealmatcherEntityBase, IAggregateRoot
{
    public User User { get; private set; } = null!;
    public Offer? Offer { get; private set; }
    public ActivityAction Action { get; private set; } = null!;
    private readonly Dictionary<string, string> _details = null!;
    public ReadOnlyDictionary<string, string> Details => _details.AsReadOnly();
    public IPAddress IPAddress { get; private set; } = null!;

    // EF
    private Activity()
    {
    }

    public Activity(User user,
                    Offer? offer,
                    ActivityAction action,
                    Dictionary<string, string> details,
                    IPAddress ipAddress)
    {
        User = user;
        Offer = offer;
        Action = action;
        _details = details;
        IPAddress = ipAddress;
    }

    public void SetDetail(string key, string value)
    {
        _details[key] = value;
    }
}
