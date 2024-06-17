using Discord.Models.Json.Stickers;
using Discord.Rest;

namespace Discord;

/// <summary>
///     Represents a custom sticker within a guild.
/// </summary>
public interface IGuildSticker :
    ISticker,
    IGuildStickerActor
{
    /// <summary>
    ///     Gets the user that uploaded the guild sticker.
    /// </summary>
    ILoadableEntity<ulong, IGuildMember>? Author { get; }
}
