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
    public Optional<ThreadArchiveDuration> DefaultAutoArchiveDuration { get; set; }
    public Optional<int?> DefaultThreadSlowmode { get; set; }
}
