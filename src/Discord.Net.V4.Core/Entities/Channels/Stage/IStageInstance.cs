using Discord.EntityRelationships;
using Discord.Models;
using Discord.Rest;

namespace Discord;

using Deletable = IDeletable<ulong, IStageInstance>;
using Modifiable = IModifiable<ulong, IStageInstance, ModifyStageInstanceProperties, ModifyStageInstanceParams>;

/// <summary>
///     Represents a live stage instance within a stage channel.
/// </summary>
public interface IStageInstance :
    ISnowflakeEntity,
    Deletable,
    Modifiable,
    IGuildRelationship,
    IChannelRelationship
{
    /// <summary>
    ///     Gets the topic of the stage.
    /// </summary>
    string Topic { get; }

    /// <summary>
    ///     Gets the stage privacy level.
    /// </summary>
    StagePrivacyLevel PrivacyLevel { get; }

    /// <summary>
    ///     Gets whether or not this stage is discoverable outside of the current guild.
    /// </summary>
    /// <remarks>
    ///     <b>Deprecated in the Discord API.</b>
    /// </remarks>
    [Obsolete("Deprecated in the Discord API")]
    bool IsDiscoverableDisabled { get; }

    /// <summary>
    ///     Gets the guild scheduled event tied to this stage instance, if any; otherwise <see langword="null" />.
    /// </summary>
    ILoadableEntity<ulong, IGuildScheduledEvent>? Event { get; }

    static ApiBodyRoute<ModifyStageInstanceParams> Modifiable.ModifyRoute(IPathable path, ulong id,
        ModifyStageInstanceParams args)
        => Routes.ModifyStageInstance(path.Require<IChannel>(), args);

    static BasicApiRoute Deletable.DeleteRoute(IPathable pathable, ulong id)
        => Routes.DeleteStageInstance(pathable.Require<IChannel>());

}
