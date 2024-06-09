namespace Discord.Models;

public interface IGuildTextChannelModel : IGuildChannelModel
{
    bool IsNsfw { get; }
    string? Topic { get; }
    int? RatelimitPerUser { get; }
    int DefaultArchiveDuration { get; }
}
