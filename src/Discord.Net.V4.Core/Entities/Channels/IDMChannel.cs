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
    IUserActor Recipient { get; }
    
    [SourceOfTruth]
    new IDMChannelModel GetModel();
}
