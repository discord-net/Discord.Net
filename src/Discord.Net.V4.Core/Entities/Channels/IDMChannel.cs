using Discord.Models;

namespace Discord;

/// <summary>
///     Represents a generic direct-message channel.
/// </summary>
public partial interface IDMChannel :
    ISnowflakeEntity<IDMChannelModel>,
    IMessageChannel,
    IUpdatable<IDMChannelModel>,
    IDMChannelActor
{
    [SourceOfTruth]
    new IDMChannelModel GetModel();
}
