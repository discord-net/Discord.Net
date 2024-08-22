using Discord.Models;
using Discord.Rest;

namespace Discord;

public sealed class ForumTag(
    IDiscordClient client,
    ulong id,
    string name,
    bool isModerated,
    DiscordEmojiId? emote
):
    ISnowflakeEntity<ITagModel>,
    IEntityProperties<Models.Json.ForumTag>,
    IModelConstructable<ForumTag, ITagModel>
{
    public string Name { get; } = name;
    public bool IsModerated { get; } = isModerated;
    public DiscordEmojiId? Emote { get; } = emote;

    public static ForumTag Construct(IDiscordClient client, ITagModel model)
    {
        return new ForumTag(
            client,
            model.Id,
            model.Name,
            model.Moderated,
            model.Emoji
        );
    }

    public Models.Json.ForumTag ToApiModel(Models.Json.ForumTag? existing = null)
    {
        existing ??= new Models.Json.ForumTag {Name = Name};
        existing.Moderated = IsModerated;
        existing.EmojiName = Emote?.Name;
        existing.EmojiId = Emote?.Id;
        existing.Id = Id;
        return existing;
    }

    public ulong Id { get; } = id;

    IDiscordClient IClientProvider.Client => client;

    public ITagModel GetModel() => ToApiModel();
}
