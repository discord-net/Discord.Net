namespace Discord.Models;

public interface IGuildTextChannelModel : IThreadableChannelModel
{
    int RatelimitPerUser { get; }
    bool IsNsfw { get; }
    string? Topic { get; }
}
