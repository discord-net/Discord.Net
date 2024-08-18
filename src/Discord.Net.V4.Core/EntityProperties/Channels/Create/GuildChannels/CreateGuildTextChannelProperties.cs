using Discord.Models.Json;

namespace Discord;

public class CreateGuildTextChannelProperties : CreateGuildChannelBaseProperties
{
    public Optional<string?> Topic { get; set; }

    public Optional<int?> Slowmode { get; set; }

    public Optional<EntityOrId<ulong, ICategoryChannel>?> Category { get; set; }

    public Optional<bool?> IsNsfw { get; set; }

    public Optional<ThreadArchiveDuration?> DefaultAutoArchiveDuration { get; set; }

    public Optional<int?> DefaultThreadSlowmode { get; set; }

    protected override Optional<ChannelType> ChannelType => Discord.ChannelType.Text;

    public override CreateGuildChannelParams ToApiModel(CreateGuildChannelParams? existing = default)
    {
        existing ??= base.ToApiModel(existing);

        existing.Topic = Topic;
        existing.RateLimitPerUser = Slowmode;
        existing.ParentId = Category.Map(v => v?.Id);
        existing.IsNsfw = IsNsfw;
        existing.DefaultAutoArchiveDuration = DefaultAutoArchiveDuration.MapToInt();
        existing.DefaultThreadRatelimitPerUser = DefaultThreadSlowmode;

        return existing;
    }
}
