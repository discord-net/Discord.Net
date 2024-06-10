namespace Discord;

public class ModifyMediaChannelProperties
{
    public Optional<string> Topic { get; set; }
    public Optional<bool?> IsNsfw { get; set; }
    public Optional<int?> Slowmode { get; set; }
    public Optional<EntityOrId<ulong, ICategoryChannel>?> CategoryId { get; set; }
    public Optional<ThreadArchiveDuration> DefaultAutoArchiveDuration { get; set; }
    public Optional<ChannelFlags> Flags { get; set; }
    public Optional<IEnumerable<EntityOrId<ulong, IForumTag>>> AvailableTags { get; set; }
    public Optional<IEmote> DefaultReaction { get; set; }
    public Optional<int> DefaultThreadSlowmode { get; set; }
    public Optional<int?> DefaultSortOrder { get; set; }
}
