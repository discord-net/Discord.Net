using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

/// <summary>
///     Represents a generic private group channel.
/// </summary>
public partial interface IGroupChannel :
    ISnowflakeEntity<IGroupDMChannelModel>,
    IMessageChannel,
    IAudioChannel,
    IGroupChannelActor
{
    [SourceOfTruth]
    new IUserActor.Defined.Indexable.BackLink<IGroupChannelActor> Recipients { get; }

    [SourceOfTruth]
    new IGroupDMChannelModel GetModel();
}
