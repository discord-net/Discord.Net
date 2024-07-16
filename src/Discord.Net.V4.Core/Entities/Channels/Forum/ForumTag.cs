using Discord.Models;
using Discord.Rest;

namespace Discord;

public sealed class ForumTag(
    IDiscordClient client,
    ulong id,
    string name,
    bool isModerated,
    IGuildEmoteActor? emote,
    string? emoji)
    : ISnowflakeEntity, IEntityProperties<Models.Json.ForumTag>,
        IContextConstructable<ForumTag, ITagModel, ForumTag.Context>
{
    public string Name { get; } = name;
    public bool IsModerated { get; } = isModerated;
    public IGuildEmoteActor? Emote { get; } = emote;
    public string? Emoji { get; } = emoji;

    public static ForumTag Construct(IDiscordClient client, ITagModel model, Context context)
    {
        var emote = model.EmojiId.HasValue
            ? client.Guild(context.GuildId).Emote(model.EmojiId.Value)
            : null;

        return new ForumTag(
            client,
            model.Id,
            model.Name,
            model.Moderated,
            emote,
            model.EmojiName
        );
    }

    public Models.Json.ForumTag ToApiModel(Models.Json.ForumTag? existing = null)
    {
        existing ??= new Models.Json.ForumTag {Name = Name};
        existing.Moderated = IsModerated;
        existing.EmojiName = Emoji;
        existing.Id = Id;
        existing.EmojiId = Emote?.Id;
        return existing;
    }

    public ulong Id { get; } = id;

    IDiscordClient IClientProvider.Client => client;

    public readonly record struct Context(ulong GuildId);
}
