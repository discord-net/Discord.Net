using Discord.Models;

namespace Discord;

public interface IThreadableChannel :
    IEntityOf<IThreadableChannelModel>,
    IGuildChannel
{
    int? DefaultThreadSlowmode { get; }

    ThreadArchiveDuration DefaultArchiveDuration { get; }

    new IThreadableChannelModel GetModel();
}
