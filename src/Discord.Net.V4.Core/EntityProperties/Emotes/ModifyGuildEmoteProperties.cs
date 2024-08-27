using Discord.Models.Json;

namespace Discord;

/// <summary>
///     Provides properties that are used to modify an <see cref="IGuildEmote" /> with the specified changes.
/// </summary>
public class ModifyGuildEmoteProperties : IEntityProperties<ModifyEmojiParams>
{
    /// <summary>
    ///     Gets or sets the name of the <see cref="IGuildEmote" />.
    /// </summary>
    public Optional<string> Name { get; set; }

    /// <summary>
    ///     Gets or sets the roles that can access this <see cref="IGuildEmote" />.
    /// </summary>
    public Optional<IEnumerable<EntityOrId<ulong, IRole>>?> Roles { get; set; }

    public ModifyEmojiParams ToApiModel(ModifyEmojiParams? existing = default)
    {
        existing ??= new ModifyEmojiParams();
        existing.Name = Name;
        existing.Roles = Roles.Map(v => v?.Select(v => v.Id).ToArray());
        return existing;
    }
}
