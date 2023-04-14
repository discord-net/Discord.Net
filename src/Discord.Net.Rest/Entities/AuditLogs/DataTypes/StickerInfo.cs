using Discord.API.AuditLogs;

namespace Discord.Rest;

/// <summary>
///     Represents information for a guild.
/// </summary>
public class StickerInfo
{
    internal StickerInfo(StickerInfoAuditLogModel model)
    {
        Name = model.Name;
        Tags = model.Tags;
        Description = model.Description;
    }

    /// <summary>
    ///     Gets the name of the sticker. <see langword="null" /> if the value was not updated in this entry.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Gets tags of the sticker. <see langword="null" /> if the value was not updated in this entry.
    /// </summary>
    public string Tags { get; set; }

    /// <summary>
    ///     Gets the description of the sticker. <see langword="null" /> if the value was not updated in this entry.
    /// </summary>
    public string Description { get; set; }
}
