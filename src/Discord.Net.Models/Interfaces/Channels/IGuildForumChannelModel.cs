namespace Discord.Models;

public interface IGuildForumChannelModel : IGuildChannelModel
{
    bool IsNsfw { get; }
    string? Topic { get; }
    int DefaultAutoArchiveDuration { get; }
    IForumTagModel[] Tags { get; }
}

public interface IForumTagModel
{
    ulong Id { get; }
    string Name { get; }
    string? EmojiName { get; }
    ulong? EmojiId { get; }
}
