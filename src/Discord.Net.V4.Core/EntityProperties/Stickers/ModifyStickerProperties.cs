using Discord.Models.Json;

namespace Discord;

/// <summary>
///     Represents a class used to modify stickers.
/// </summary>
public class ModifyStickerProperties : IEntityProperties<ModifyGuildStickerParams>
{
    /// <summary>
    ///     Gets or sets the name of the sticker.
    /// </summary>
    public Optional<string> Name { get; set; }

    /// <summary>
    ///     Gets or sets the description of the sticker.
    /// </summary>
    public Optional<string?> Description { get; set; }

    /// <summary>
    ///     Gets or sets the tags of the sticker.
    /// </summary>
    public Optional<CSVString> Tags { get; set; }

    public ModifyGuildStickerParams ToApiModel(ModifyGuildStickerParams? existing = default)
    {
        existing ??= new ModifyGuildStickerParams();

        existing.Name = Name;
        existing.Description = Description;
        existing.Tags = Tags.Map(v => v.Value);

        return existing;
    }
}
