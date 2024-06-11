namespace Discord;

public sealed class ForumTag(
    IDiscordClient client,
    ulong id,
    string name,
    bool isModerated,
    ILoadableEntity<ulong, IGuildEmote>? emote,
    string? emoji)
    : ISnowflakeEntity, IEntityProperties<Models.Json.ForumTag>, IConstructable<ForumTag, Models.IForumTagModel, ForumTag.Context>
{
    public readonly record struct Context(ulong GuildId);

    public ulong Id { get; } = id;
    public string Name { get; } = name;
    public bool IsModerated { get; } = isModerated;
    public ILoadableEntity<ulong, IGuildEmote>? Emote { get; } = emote;
    public string? Emoji { get; } = emoji;

    IDiscordClient IClientProvider.Client => client;

    public Models.Json.ForumTag ToApiModel(Models.Json.ForumTag? existing = null)
    {
        existing ??= new() {Name = Name};
        existing.Moderated = IsModerated;
        existing.EmojiName = Emoji;
        existing.Id = Id;
        existing.EmojiId = Emote?.Id;
        return existing;
    }

    public static ForumTag Construct(IDiscordClient client, Models.IForumTagModel model, Context context)
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
}
