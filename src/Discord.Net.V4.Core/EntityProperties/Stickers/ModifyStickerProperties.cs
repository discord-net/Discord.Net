namespace Discord;

/// <summary>
///     Represents a class used to modify stickers.
/// </summary>
public class ModifyStickerProperties
{
    /// <summary>
    ///     Gets or sets the name of the sticker.
    /// </summary>
    public Optional<string> Name { get; set; }

    /// <summary>
    ///     Gets or sets the description of the sticker.
    /// </summary>
    public Optional<string> Description { get; set; }

    /// <summary>
    ///     Gets or sets the tags of the sticker.
    /// </summary>
    public Optional<CSVString> Tags { get; set; }
}
