using Discord.Integration;

namespace Discord;

public interface IIntegration : IEntity<ulong>, IIntegrationActor
{
    string Name { get; }
    IntegrationType Type { get; }
    bool IsEnabled { get; }
    bool? IsSyncing { get; }
    IRoleActor? Role { get; }
    bool? EmoticonsEnabled { get; }
    IntegrationExpireBehavior ExpireBehavior { get; }
    int? ExpiryGracePeriod { get; }
    ILoadableUserActor? User { get; }
    IntegrationAccount? Account { get; }
    DateTimeOffset? SyncedAt { get; }
    int? SubscriberCount { get; }
    bool? IsRevoked { get; }
    IntegrationApplication? Application { get; }
    string[] Scopes { get; }
}
