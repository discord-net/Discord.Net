using Discord.Models;
using Discord.Rest;
using Discord.Stage;

namespace Discord;

using IModifiable = IModifiable<ulong, IStageInstance, ModifyStageInstanceProperties, ModifyStageInstanceParams, IStageInstanceModel>;

/// <summary>
///     Represents a live stage instance within a stage channel.
/// </summary>
public interface IStageInstance :
    ISnowflakeEntity,
    IStageInstanceActor,
    IRefreshable<IStageInstance, ulong, IStageInstanceModel>,
    IModifiable
{
    static IApiInOutRoute<ModifyStageInstanceParams, IEntityModel> IModifiable.ModifyRoute(
        IPathable path,
        ulong id,
        ModifyStageInstanceParams args
    ) => Routes.ModifyStageInstance(path.Require<IChannel>(), args);

    static IApiOutRoute<IStageInstanceModel> IRefreshable<IStageInstance, ulong, IStageInstanceModel>.RefreshRoute(
        IPathable path,
        ulong id
    ) => Routes.GetStageInstance(path.Require<IChannel>());

    /// <summary>
    ///     Gets the topic of the stage.
    /// </summary>
    string Topic { get; }

    /// <summary>
    ///     Gets the stage privacy level.
    /// </summary>
    StagePrivacyLevel PrivacyLevel { get; }

    /// <summary>
    ///     Gets the guild scheduled event tied to this stage instance, if any; otherwise <see langword="null" />.
    /// </summary>
    ILoadableEntity<ulong, IGuildScheduledEvent>? Event { get; }
}
