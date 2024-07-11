using Discord.Models;

namespace Discord;

public partial interface IThreadableChannel :
    IEntityOf<IThreadableChannelModel>,
    IGuildChannel
{
    int? DefaultThreadSlowmode { get; }

    ThreadArchiveDuration DefaultArchiveDuration { get; }

    [SourceOfTruth]
    new IThreadableChannelModel GetModel();


}
