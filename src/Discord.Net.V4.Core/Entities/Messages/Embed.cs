using System.Collections.Immutable;
using System.Diagnostics;

namespace Discord;

/// <summary>
///     Represents an embed object seen in an <see cref="IUserMessage"/>.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class Embed : IEmbed
{
    /// <inheritdoc/>
    public EmbedType Type { get; }

    /// <inheritdoc/>
    public string? Description { get; }

    /// <inheritdoc/>
    public string? Url { get; }

    /// <inheritdoc/>
    public string? Title { get;  }

    /// <inheritdoc/>
    public DateTimeOffset? Timestamp { get; }

    /// <inheritdoc/>
    public Color? Color { get; }

    /// <inheritdoc/>
    public EmbedImage? Image { get; }

    /// <inheritdoc/>
    public EmbedVideo? Video { get; }

    /// <inheritdoc/>
    public EmbedAuthor? Author { get; }

    /// <inheritdoc/>
    public EmbedFooter? Footer { get; }

    /// <inheritdoc/>
    public EmbedProvider? Provider { get; }

    /// <inheritdoc/>
    public EmbedThumbnail? Thumbnail { get; }

    /// <inheritdoc/>
    public ImmutableArray<EmbedField> Fields { get; }

    internal Embed(EmbedType type)
    {
        Type = type;
        Fields = ImmutableArray.Create<EmbedField>();
    }

    internal Embed(EmbedType type,
        string? title,
        string? description,
        string? url,
        DateTimeOffset? timestamp,
        Color? color,
        EmbedImage? image,
        EmbedVideo? video,
        EmbedAuthor? author,
        EmbedFooter? footer,
        EmbedProvider? provider,
        EmbedThumbnail? thumbnail,
        ImmutableArray<EmbedField> fields)
    {
        Type = type;
        Title = title;
        Description = description;
        Url = url;
        Color = color;
        Timestamp = timestamp;
        Image = image;
        Video = video;
        Author = author;
        Footer = footer;
        Provider = provider;
        Thumbnail = thumbnail;
        Fields = fields;
    }

    /// <summary>
    ///     Gets the total length of all embed properties.
    /// </summary>
    public int Length
    {
        get
        {
            int titleLength = Title?.Length ?? 0;
            int authorLength = Author?.Name?.Length ?? 0;
            int descriptionLength = Description?.Length ?? 0;
            int footerLength = Footer?.Text?.Length ?? 0;
            int fieldSum = Fields.Sum(f => f.Name?.Length + f.Value?.ToString().Length) ?? 0;
            return titleLength + authorLength + descriptionLength + footerLength + fieldSum;
        }
    }

    /// <summary>
    ///     Gets the title of the embed.
    /// </summary>
    public override string? ToString() => Title;

    private string DebuggerDisplay => $"{Title} ({Type})";

    public static bool operator ==(Embed? left, Embed? right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(Embed? left, Embed? right)
        => !(left == right);

    /// <summary>
    /// Determines whether the specified object is equal to the current <see cref="Embed"/>.
    /// </summary>
    /// <remarks>
    /// If the object passes is an <see cref="Embed"/>, <see cref="Equals(Embed)"/> will be called to compare the 2 instances
    /// </remarks>
    /// <param name="obj">The object to compare with the current <see cref="Embed"/></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
        => obj is Embed embed && Equals(embed);

    /// <summary>
    /// Determines whether the specified <see cref="Embed"/> is equal to the current <see cref="Embed"/>
    /// </summary>
    /// <param name="embed">The <see cref="Embed"/> to compare with the current <see cref="Embed"/></param>
    /// <returns></returns>
    public bool Equals(Embed? embed)
        => GetHashCode() == embed?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            hash = hash * 23 + (Type, Title, Description, Timestamp, Color, Image, Video, Author, Footer, Provider, Thumbnail).GetHashCode();
            foreach (var field in Fields)
                hash = hash * 23 + field.GetHashCode();
            return hash;
        }
    }
}
