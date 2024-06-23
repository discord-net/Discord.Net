using Discord.Models.Json.Stickers;

namespace Discord;

/// <summary>
///     Represents a class used to modify stickers.
/// </summary>
public class ModifyStickerProperties : IEntityProperties<ModifyGuildStickersParams>
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

    public ModifyGuildStickersParams ToApiModel(ModifyGuildStickersParams? existing = default)
    {
        existing ??= new ModifyGuildStickersParams();

        existing.Name = Name;
        existing.Description = Description;
        existing.Tags = Tags.Map(v => v.Value);

        return existing;
    }
}
