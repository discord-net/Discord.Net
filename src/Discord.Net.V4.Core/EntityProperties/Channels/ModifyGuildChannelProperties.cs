using Discord.Models.Json;

namespace Discord;

/// <summary>
///     Properties that are used to modify an <see cref="IGuildChannel" /> with the specified changes.
/// </summary>
public class ModifyGuildChannelProperties : ModifyChannelBaseProperties, IEntityProperties<ModifyGuildChannelParams>
{
    /// <summary>
    ///     Moves the channel to the following position. This property is zero-based.
    /// </summary>
    public Optional<int?> Position { get; set; }

    /// <summary>
    ///     Gets or sets the permission overwrites for this channel.
    /// </summary>
    public Optional<IEnumerable<Overwrite>> PermissionOverwrites { get; set; }

    public virtual ModifyGuildChannelParams ToApiModel(ModifyGuildChannelParams? existing = null)
    {
        existing ??= new();
        base.ToApiModel(existing);
        existing.Position = Position;
        existing.PermissionOverwrites = PermissionOverwrites.Map(v => v.Select(x => x.ToApiModel()).ToArray());
        return existing;
    }
}
