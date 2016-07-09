using System;

namespace Discord
{
    //TODO: Add docstrings
    public interface IGuildIntegration
    {
        ulong Id { get; }
        string Name { get; }
        string Type { get; }
        bool IsEnabled { get; }
        bool IsSyncing { get; }
        ulong ExpireBehavior { get; }
        ulong ExpireGracePeriod { get; }
        DateTimeOffset SyncedAt { get; }
        IntegrationAccount Account { get; }

        IGuild Guild { get; }
        IUser User { get; }
        IRole Role { get; }
    }
}
