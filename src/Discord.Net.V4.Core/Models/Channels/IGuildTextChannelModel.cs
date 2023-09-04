using Discord.Entities.Channels.Threads;

namespace Discord.Models;

public interface IGuildTextChannelModel : IGuildChannelModel
{
    bool IsNsfw { get; }
    string? Topic { get; }
    int Slowmode { get; }
    ThreadArchiveDuration DefaultArchiveDuration { get; }
}
