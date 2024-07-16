using Discord.Models;

namespace Discord;

/// <summary>
///     Represents a generic Stage Channel.
/// </summary>
public partial interface IStageChannel :
    ISnowflakeEntity<IGuildStageChannelModel>,
    IVoiceChannel,
    IStageChannelActor,
    IUpdatable<IGuildStageChannelModel>
{
    [SourceOfTruth]
    new IGuildStageChannelModel GetModel();
}
