using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public partial interface IThreadableChannel :
    IGuildChannel,
    IThreadableChannelActor,
    IEntityOf<IThreadableChannelModel>
{
    [SourceOfTruth]
    new IThreadableChannelModel GetModel();

    int? DefaultThreadSlowmode { get; }

    ThreadArchiveDuration DefaultArchiveDuration { get; }
}
