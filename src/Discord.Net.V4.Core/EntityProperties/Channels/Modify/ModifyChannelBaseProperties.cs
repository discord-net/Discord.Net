using Discord.Models.Json;

namespace Discord;

public abstract class ModifyChannelBaseProperties : IEntityProperties<ModifyChannelParams>
{
    /// <summary>
    ///     Gets or sets the channel to this name.
    /// </summary>
    /// <remarks>
    ///     This property defines the new name for this channel.
    ///     <note type="warning">
    ///         When modifying an <see cref="ITextChannel" />, the <see cref="Name" /> must be alphanumeric with
    ///         dashes. It must match the RegEx <c>[a-z0-9-_]{2,100}</c>.
    ///     </note>
    /// </remarks>
    public Optional<string> Name { get; set; }

    public virtual ModifyChannelParams ToApiModel(ModifyChannelParams? existing = default)
    {
        existing ??= new ModifyChannelParams();

        existing.Name = Name;

        return existing;
    }
}
