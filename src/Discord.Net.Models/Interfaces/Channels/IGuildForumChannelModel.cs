using Discord.Models.Json;

namespace Discord.Models;

[ModelEquality]
public partial interface IGuildForumChannelModel : IThreadableChannelModel
{
    bool IsNsfw { get; }
    string? Topic { get; }
    int? RatelimitPerUser { get; }
    ulong? DefaultReactionEmojiId { get; }
    string? DefaultReactionEmojiName { get; }
    IEnumerable<ITagModel> AvailableTags { get; }
    int? DefaultSortOrder { get; }
    int DefaultForumLayout { get; }
}
