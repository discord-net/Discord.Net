using Discord.Models.Json;

namespace Discord.Models;

public interface IGuildMediaChannelModel : IThreadableChannelModel
{
    bool IsNsfw { get; }
    string? Topic { get; }
    int? RatelimitPerUser { get; }
    IEmote? DefaultReactionEmoji { get; }
    IEnumerable<ITagModel> AvailableTags { get; }
    int? DefaultSortOrder { get; }
}
