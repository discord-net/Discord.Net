using Discord.Models;

namespace Discord;

/// <summary>
///     Represents a generic Stage Channel.
/// </summary>
public partial interface IStageChannel :
    IVoiceChannel,
    IStageChannelActor,
    IUpdatable<IGuildStageChannelModel>;
