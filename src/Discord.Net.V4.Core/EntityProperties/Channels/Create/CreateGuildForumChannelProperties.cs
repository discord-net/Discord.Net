using Discord.Models.Json;

namespace Discord;

public class CreateGuildForumChannelProperties : CreateGuildChannelProperties
{
    public Optional<string?> Topic { get; set; }
    public Optional<int?> Slowmode { get; set; }
    public Optional<EntityOrId<ulong, ICategoryChannel>?> Category { get; set; }
    public Optional<bool?> IsNsfw { get; set; }
    public Optional<ThreadArchiveDuration?> DefaultAutoArchiveDuration { get; set; }
    public Optional<IEmote?> DefaultReactionEmoji { get; set; }
    public Optional<ForumTag[]?> AvailableTags { get; set; }
    public Optional<ForumSortOrder?> DefaultSortOrder { get; set; }
    public Optional<ForumLayout?> DefaultForumLayout { get; set; }
    public Optional<int?> DefaultThreadSlowmode { get; set; }

    public override CreateGuildChannelParams ToApiModel(CreateGuildChannelParams? existing = default)
    {
        existing ??= base.ToApiModel(existing);

        existing.Topic = Topic;
        existing.RateLimitPerUser = Slowmode;
        existing.ParentId = Category.MapToNullableId();
        existing.IsNsfw = IsNsfw;
        existing.DefaultAutoArchiveDuration = DefaultAutoArchiveDuration.MapToInt();
        existing.DefaultReactionEmoji = DefaultReactionEmoji.Map(v => v?.ToDefaultReactionModel());
        existing.AvailableTags = AvailableTags.Map(v => v?.Select(x => x.ToApiModel()).ToArray());
        existing.DefaultSortOrder = DefaultSortOrder.MapToInt();
        existing.DefaultForumLayout = DefaultForumLayout.MapToInt();
        existing.DefaultThreadRatelimitPerUser = DefaultThreadSlowmode;

        return existing;
    }
}
