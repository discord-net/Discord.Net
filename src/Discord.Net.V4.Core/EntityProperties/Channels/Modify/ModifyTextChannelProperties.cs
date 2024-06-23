using Discord.Models.Json;

namespace Discord;

/// <summary>
///     Provides properties that are used to modify an <see cref="ITextChannel" /> with the specified changes.
/// </summary>
public class ModifyTextChannelProperties : ModifyGuildChannelProperties
{
    public Optional<ChannelType> Type { get; set; }
    public Optional<string?> Topic { get; set; }
    public Optional<int?> Slowmode { get; set; }
    public Optional<EntityOrId<ulong, ICategoryChannel>?> Category { get; set; }
    public Optional<ThreadArchiveDuration?> DefaultAutoArchiveDuration { get; set; }
    public Optional<int?> DefaultThreadSlowmode { get; set; }

    public override ModifyGuildChannelParams ToApiModel(ModifyGuildChannelParams? existing = null)
    {
        existing ??= new ModifyGuildChannelParams();
        base.ToApiModel(existing);

        existing.Type = Type.Map(v => (int)v);
        existing.Topic = Topic;
        existing.RateLimitPerUser = Slowmode;
        existing.ParentId = Category.Map(v => v?.Id);
        existing.DefaultAutoArchiveDuration = DefaultAutoArchiveDuration.Map(v => (int?)v);
        existing.DefaultThreadRateLimitPerUser = DefaultThreadSlowmode;

        return existing;
    }
}
