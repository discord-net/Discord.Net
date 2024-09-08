using Discord.Models.Json;

namespace Discord.Models;

[ModelEquality]
public partial interface IGuildMediaChannelModel : IThreadableChannelModel
{
    bool IsNsfw { get; }
    string? Topic { get; }
    int? RatelimitPerUser { get; }
    DiscordEmojiId? DefaultReactionEmoji { get; }
    IEnumerable<ITagModel> AvailableTags { get; }
    int? DefaultSortOrder { get; }
}
