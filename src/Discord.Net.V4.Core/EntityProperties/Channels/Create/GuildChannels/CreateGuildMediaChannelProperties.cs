using Discord.Models.Json;

namespace Discord;

public class CreateGuildMediaChannel : CreateGuildChannelBaseProperties
{
    public Optional<string?> Topic { get; set; }
    public Optional<int?> Slowmode { get; set; }
    public Optional<EntityOrId<ulong, ICategoryChannel>?> Category { get; set; }
    public Optional<bool?> IsNsfw { get; set; }
    public Optional<ThreadArchiveDuration?> DefaultAutoArchiveDuration { get; set; }
    public Optional<IEmote?> DefaultReactionEmoji { get; set; }
    public Optional<ForumTag[]?> AvailableTags { get; set; }
    public Optional<SortOrder?> DefaultSortOrder { get; set; }
    public Optional<int?> DefaultThreadSlowmode { get; set; }

    protected override Optional<ChannelType> ChannelType => Discord.ChannelType.Media;

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
        existing.DefaultThreadRatelimitPerUser = DefaultThreadSlowmode;

        return existing;
    }
}
