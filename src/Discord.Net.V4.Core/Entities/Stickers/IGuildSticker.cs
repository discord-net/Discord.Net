using System;

namespace Discord;

/// <summary>
///     Represents a custom sticker within a guild.
/// </summary>
public interface IGuildSticker : ISticker, IModifyable<StickerProperties>, IDeletable
{
    /// <summary>
    ///     Gets the user that uploaded the guild sticker.
    /// </summary>
    IEntitySource<ulong, IGuildUser>? Author { get; }

    /// <summary>
    ///     Gets the guild that this custom sticker is in.
    /// </summary>
    IEntitySource<ulong, IGuild> Guild { get; }
}
