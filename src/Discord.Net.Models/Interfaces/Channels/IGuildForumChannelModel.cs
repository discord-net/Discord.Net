namespace Discord.Models;

public interface IGuildForumChannelModel : IGuildChannelModel
{
    bool IsNsfw { get; }
    string? Topic { get; }
    int DefaultAutoArchiveDuration { get; }
    IEnumerable<IForumTagModel> AvailableTags { get; }
}

public interface IForumTagModel
{
    ulong Id { get; }
    string Name { get; }
    bool Moderated { get; }
    string? EmojiName { get; }
    ulong? EmojiId { get; }
}
