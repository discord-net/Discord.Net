using System;

namespace Discord
{
    public interface IGuildIntegration
    {
        ulong Id { get; }
        string Name { get; }
        string Type { get; }
        bool IsEnabled { get; }
        bool IsSyncing { get; }
        ulong ExpireBehavior { get; }
        ulong ExpireGracePeriod { get; }
        DateTime SyncedAt { get; }
        IntegrationAccount Account { get; }

        IGuild Guild { get; }
        IUser User { get; }
        IRole Role { get; }
    }
}
