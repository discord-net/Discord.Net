using Discord.Models;
using System.Diagnostics;

namespace Discord;

/// <summary>
///     A video featured in an <see cref="Embed" />.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct EmbedVideo : IEntityProperties<Models.Json.EmbedVideo>,
    IConstructable<EmbedVideo, IEmbedVideoModel>
{
    /// <summary>
    ///     The URL of the video.
    /// </summary>
    /// <returns>
    ///     A string containing the URL of the image.
    /// </returns>
    public readonly string? Url;

    public readonly string? ProxyUrl;

    /// <summary>
    ///     The height of the video.
    /// </summary>
    /// <returns>
    ///     A <see cref="int" /> representing the height of this video if it can be retrieved; otherwise
    ///     <see langword="null" />.
    /// </returns>
    public readonly int? Height;

    /// <summary>
    ///     The weight of the video.
    /// </summary>
    /// <returns>
    ///     A <see cref="int" /> representing the width of this video if it can be retrieved; otherwise
    ///     <see langword="null" />.
    /// </returns>
    public readonly int? Width;

    internal EmbedVideo(string? url, string? proxyUrl, int? height, int? width)
    {
        Url = url;
        ProxyUrl = proxyUrl;
        Height = height;
        Width = width;
    }

    private string DebuggerDisplay => $"{Url} ({(Width != null && Height != null ? $"{Width}x{Height}" : "0x0")})";

    /// <summary>
    ///     Gets the URL of the video.
    /// </summary>
    /// <returns>
    ///     A string that resolves to <see cref="Url" />.
    /// </returns>
    public override string ToString() => Url ?? ProxyUrl ?? "<unknown>";

    public static bool operator ==(EmbedVideo? left, EmbedVideo? right)
        => left is null
            ? right is null
            : left.Equals(right);

    public static bool operator !=(EmbedVideo? left, EmbedVideo? right)
        => !(left == right);

    public Models.Json.EmbedVideo ToApiModel(Models.Json.EmbedVideo? existing = default)
    {
        existing ??= new Models.Json.EmbedVideo();

        existing.Url = Optional.FromNullable(Url);
        existing.ProxyUrl = Optional.FromNullable(ProxyUrl);
        existing.Height = Optional.FromNullable(Height);
        existing.Width = Optional.FromNullable(Width);

        return existing;
    }

    public static EmbedVideo Construct(IDiscordClient client, IEmbedVideoModel model)
        => new(model.Url, model.ProxyUrl, model.Height, model.Width);

    /// <summary>
    ///     Determines whether the specified object is equal to the current <see cref="EmbedVideo" />.
    /// </summary>
    /// <remarks>
    ///     If the object passes is an <see cref="EmbedVideo" />, <see cref="Equals(EmbedVideo?)" /> will be called to compare
    ///     the 2 instances
    /// </remarks>
    /// <param name="obj">The object to compare with the current <see cref="EmbedVideo" /></param>
    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is EmbedVideo embedVideo && Equals(embedVideo);

    /// <summary>
    ///     Determines whether the specified <see cref="EmbedVideo" /> is equal to the current <see cref="EmbedVideo" />
    /// </summary>
    /// <param name="embedVideo">The <see cref="EmbedVideo" /> to compare with the current <see cref="EmbedVideo" /></param>
    /// <inheritdoc cref="Equals(object?)" />
    public bool Equals(EmbedVideo? embedVideo)
        => GetHashCode() == embedVideo?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
        => (Width, Height, Url).GetHashCode();
}
