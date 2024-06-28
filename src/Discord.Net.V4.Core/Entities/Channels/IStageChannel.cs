using Discord.Models;

namespace Discord;

/// <summary>
///     Represents a generic Stage Channel.
/// </summary>
public interface IStageChannel :
    IVoiceChannel,
    IStageChannelActor,
    IUpdatable<IGuildStageChannelModel>
{
}
