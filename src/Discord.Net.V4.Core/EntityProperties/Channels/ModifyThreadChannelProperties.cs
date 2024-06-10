namespace Discord;

/// <summary>
///     Provides properties that are used to modify an <see cref="IThreadChannel" /> with the specified changes.
/// </summary>
public class ModifyThreadChannelProperties : ModifyChannelBaseProperties
{
    public Optional<bool> IsArchived { get; set; }
    public Optional<ThreadArchiveDuration> AutoArchiveDuration { get; set; }
    public Optional<bool> IsLocked { get; set; }
    public Optional<bool> IsInvitable { get; set; }
    public Optional<int?> Slowmode { get; set; }
    public Optional<ChannelFlags> Flags { get; set; }
    public Optional<IEnumerable<EntityOrId<ulong, IForumTag>>> AppliedTags { get; set; }
}
