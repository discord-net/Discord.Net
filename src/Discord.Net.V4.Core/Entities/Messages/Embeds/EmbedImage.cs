using Discord.Models;
using System.Diagnostics;

namespace Discord;

/// <summary> An image for an <see cref="Embed" />. </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly struct EmbedImage :
    IEntityProperties<Models.Json.EmbedImage>,
    IModelConstructable<EmbedImage, IEmbedImageModel>
{
    /// <summary>
    ///     The URL of the image.
    /// </summary>
    /// <returns>
    ///     A string containing the URL of the image.
    /// </returns>
    public readonly string Url;

    /// <summary>
    ///     A proxied URL of this image.
    /// </summary>
    /// <returns>
    ///     A string containing the proxied URL of this image.
    /// </returns>
    public readonly string? ProxyUrl;

    /// <summary>
    ///     The height of this image.
    /// </summary>
    /// <returns>
    ///     A <see cref="int" /> representing the height of this image if it can be retrieved; otherwise
    ///     <see langword="null" />.
    /// </returns>
    public readonly int? Height;

    /// <summary>
    ///     The width of this image.
    /// </summary>
    /// <returns>
    ///     A <see cref="int" /> representing the width of this image if it can be retrieved; otherwise
    ///     <see langword="null" />.
    /// </returns>
    public readonly int? Width;

    internal EmbedImage(string url, string? proxyUrl, int? height, int? width)
    {
        Url = url;
        ProxyUrl = proxyUrl;
        Height = height;
        Width = width;
    }

    private string DebuggerDisplay => $"{Url} ({(Width != null && Height != null ? $"{Width}x{Height}" : "0x0")})";

    /// <summary>
    ///     Gets the URL of the thumbnail.
    /// </summary>
    /// <returns>
    ///     A string that resolves to <see cref="Discord.EmbedImage.Url" /> .
    /// </returns>
    public override string? ToString() => Url;

    public static bool operator ==(EmbedImage? left, EmbedImage? right)
        => left is null
            ? right is null
            : left.Equals(right);

    public static bool operator !=(EmbedImage? left, EmbedImage? right)
        => !(left == right);

    public Models.Json.EmbedImage ToApiModel(Models.Json.EmbedImage? existing = default)
    {
        existing ??= new Models.Json.EmbedImage {Url = Url};

        existing.ProxyUrl = Optional.FromNullable(ProxyUrl);
        existing.Height = Optional.FromNullable(Height);
        existing.Width = Optional.FromNullable(Width);

        return existing;
    }

    public static EmbedImage Construct(IDiscordClient client, IEmbedImageModel model) =>
        new(model.Url, model.ProxyUrl, model.Height, model.Width);

    /// <summary>
    ///     Determines whether the specified object is equal to the current <see cref="EmbedImage" />.
    /// </summary>
    /// <remarks>
    ///     If the object passes is an <see cref="EmbedImage" />, <see cref="Equals(EmbedImage?)" /> will be called to compare
    ///     the 2 instances
    /// </remarks>
    /// <param name="obj">The object to compare with the current <see cref="EmbedImage" /></param>
    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is EmbedImage embedImage && Equals(embedImage);

    /// <summary>
    ///     Determines whether the specified <see cref="EmbedImage" /> is equal to the current <see cref="EmbedImage" />
    /// </summary>
    /// <param name="embedImage">The <see cref="EmbedImage" /> to compare with the current <see cref="EmbedImage" /></param>
    /// <inheritdoc cref="Object.Equals(object?)" />
    public bool Equals(EmbedImage? embedImage)
        => GetHashCode() == embedImage?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
        => (Height, Width, Url, ProxyUrl).GetHashCode();
}
