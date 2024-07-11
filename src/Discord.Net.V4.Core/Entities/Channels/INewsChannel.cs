using Discord.Models;

namespace Discord;

/// <summary>
///     Represents a generic news channel in a guild that can send and receive messages.
/// </summary>
public partial interface INewsChannel :
    ITextChannel,
    INewsChannelActor,
    IUpdatable<IGuildNewsChannelModel>
{
    [SourceOfTruth]
    new IGuildNewsChannelModel GetModel();


}
