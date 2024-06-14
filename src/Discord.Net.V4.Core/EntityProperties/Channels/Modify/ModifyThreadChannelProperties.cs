using Discord.Models.Json;

namespace Discord;

/// <summary>
///     Provides properties that are used to modify an <see cref="IThreadChannel" /> with the specified changes.
/// </summary>
public class ModifyThreadChannelProperties : ModifyChannelBaseProperties, IEntityProperties<ModifyThreadChannelParams>
{
    public Optional<bool> IsArchived { get; set; }
    public Optional<ThreadArchiveDuration> AutoArchiveDuration { get; set; }
    public Optional<bool> IsLocked { get; set; }
    public Optional<bool> IsInvitable { get; set; }
    public Optional<int?> Slowmode { get; set; }
    public Optional<ChannelFlags> Flags { get; set; }
    public Optional<IEnumerable<ForumTag>> AppliedTags { get; set; }

    public ModifyThreadChannelParams ToApiModel(ModifyThreadChannelParams? existing = default)
    {
        existing ??= new();
        base.ToApiModel(existing);

        existing.IsArchived = IsArchived;
        existing.AutoArchiveDuration = AutoArchiveDuration.Map(v => (int)v);
        existing.IsLocked = IsLocked;
        existing.IsInvitable = IsInvitable;
        existing.RateLimitPerUser = Slowmode;
        existing.Flags = Flags.Map(v => (int)v);
        existing.AppliedTags = AppliedTags.Map(v => v.Select(v => v.ToApiModel()).ToArray());

        return existing;
    }
}
