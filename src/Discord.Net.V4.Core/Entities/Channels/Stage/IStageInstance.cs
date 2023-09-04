namespace Discord;

public interface IStageInstance : ISnowflakeEntity, IDeletable, IModifyable<StageInstanceProperties>
{
    IEntitySource<ulong, IGuild> Guild { get; }

    IEntitySource<ulong, IStageChannel> Channel { get; }

    string Topic { get; }

    StagePrivacyLevel PrivacyLevel { get; }

    [Obsolete("Deprecated in the Discord API")]
    bool IsDiscoverableDisabled { get; }

    IEntitySource<ulong, IGuildScheduledEvent>? Event { get; }
}
