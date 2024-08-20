using Discord.Models.Json;

namespace Discord;

public class ModifyMediaChannelProperties : ModifyThreadableChannelProperties
{
    public Optional<string?> Topic { get; set; }
    public Optional<bool?> IsNsfw { get; set; }
    public Optional<int?> Slowmode { get; set; }
    public Optional<EntityOrId<ulong, ICategoryChannel>?> CategoryId { get; set; }
    public Optional<ChannelFlags> Flags { get; set; }
    public Optional<IEnumerable<ForumTag>> AvailableTags { get; set; }
    public Optional<IEmote?> DefaultReactionEmoji { get; set; }
    public Optional<int?> DefaultSortOrder { get; set; }

    public override ModifyGuildChannelParams ToApiModel(ModifyGuildChannelParams? existing = null)
    {
        existing ??= new ModifyGuildChannelParams();
        base.ToApiModel(existing);

        existing.Topic = Topic;
        existing.Nsfw = IsNsfw;
        existing.RateLimitPerUser = Slowmode;
        existing.ParentId = CategoryId.Map(v => v?.Id);
        existing.Flags = Flags.Map(v => (int)v);
        existing.AvailableTags = AvailableTags.Map(v => v.Select(v => v.ToApiModel()).ToArray());
        existing.DefaultReactionEmoji = DefaultReactionEmoji.Map(v => v?.Id);
        existing.DefaultForumSortOrder = DefaultSortOrder.Map(v => v);

        return existing;
    }
}
