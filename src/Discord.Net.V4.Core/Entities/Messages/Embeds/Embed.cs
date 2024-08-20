using Discord.Models;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Discord;

/// <summary>
///     Represents an embed object seen in an <see cref="IMessage" />.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public sealed class Embed : IEntityProperties<Models.Json.Embed>, IModelConstructable<Embed, IEmbedModel>
{
    internal Embed(EmbedType type)
    {
        Type = type;
        Fields = [];
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
        IEnumerable<EmbedField> fields)
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
        Fields = fields.ToImmutableArray();
    }

    public EmbedType Type { get; }


    public string? Description { get; }


    public string? Url { get; }


    public string? Title { get; }


    public DateTimeOffset? Timestamp { get; }


    public Color? Color { get; }


    public EmbedImage? Image { get; }


    public EmbedVideo? Video { get; }


    public EmbedAuthor? Author { get; }


    public EmbedFooter? Footer { get; }


    public EmbedProvider? Provider { get; }


    public EmbedThumbnail? Thumbnail { get; }


    public IReadOnlyCollection<EmbedField> Fields { get; }

    /// <summary>
    ///     Gets the total length of all embed properties.
    /// </summary>
    public int Length
    {
        get
        {
            var titleLength = Title?.Length ?? 0;
            var authorLength = Author?.Name?.Length ?? 0;
            var descriptionLength = Description?.Length ?? 0;
            var footerLength = Footer?.Text?.Length ?? 0;
            var fieldSum = Fields.Sum(f => f.Name?.Length + f.Value?.ToString().Length) ?? 0;
            return titleLength + authorLength + descriptionLength + footerLength + fieldSum;
        }
    }

    private string DebuggerDisplay => $"{Title} ({Type})";

    public static Embed Construct(IDiscordClient client, IEmbedModel model) =>
        new(
            model.Type is not null ? new EmbedType(model.Type) : EmbedType.Unspecified,
            model.Title,
            model.Description,
            model.Url,
            model.Timestamp,
            model.Color is not null ? new Color(model.Color.Value) : null,
            model.Image is not null ? EmbedImage.Construct(client, model.Image) : null,
            model.Video is not null ? EmbedVideo.Construct(client, model.Video) : null,
            model.Author is not null ? EmbedAuthor.Construct(client, model.Author) : null,
            model.Footer is not null ? EmbedFooter.Construct(client, model.Footer) : null,
            model.Provider is not null ? EmbedProvider.Construct(client, model.Provider) : null,
            model.Thumbnail is not null ? EmbedThumbnail.Construct(client, model.Thumbnail) : null,
            model.Fields is not null
                ? model.Fields.Select(x => EmbedField.Construct(client, x)).ToArray()
                : []
        );

    public Models.Json.Embed ToApiModel(Models.Json.Embed? existing = default)
    {
        existing ??= new Models.Json.Embed();

        existing.Type = Optional.FromNullable<string>(Type);
        existing.Description = Optional.FromNullable(Description);
        existing.Url = Optional.FromNullable(Url);
        existing.Title = Optional.FromNullable(Title);
        existing.Timestamp = Timestamp;
        existing.Color = Optional.FromNullable(Color).Map(v => v.RawValue);
        existing.Image = Optional.FromNullable(Image).Map(v => v.ToApiModel());
        existing.Video = Optional.FromNullable(Video).Map(v => v.ToApiModel());
        existing.Author = Optional.FromNullable(Author).Map(v => v.ToApiModel());
        existing.Footer = Optional.FromNullable(Footer).Map(v => v.ToApiModel());
        existing.Provider = Optional.FromNullable(Provider).Map(v => v.ToApiModel());
        existing.Thumbnail = Optional.FromNullable(Thumbnail).Map(v => v.ToApiModel());
        existing.Fields = Fields.Count == 0
            ? Optional<Models.Json.EmbedField[]>.Unspecified
            : new Optional<Models.Json.EmbedField[]>(Fields.Select(v => v.ToApiModel()).ToArray());

        return existing;
    }

    /// <summary>
    ///     Gets the title of the embed.
    /// </summary>
    public override string? ToString() => Title;

    public static bool operator ==(Embed? left, Embed? right)
        => left?.Equals(right) ?? right is null;

    public static bool operator !=(Embed? left, Embed? right)
        => !(left == right);

    /// <summary>
    ///     Determines whether the specified object is equal to the current <see cref="Embed" />.
    /// </summary>
    /// <remarks>
    ///     If the object passes is an <see cref="Embed" />, <see cref="Equals(Embed)" /> will be called to compare the 2
    ///     instances
    /// </remarks>
    /// <param name="obj">The object to compare with the current <see cref="Embed" /></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
        => obj is Embed embed && Equals(embed);

    /// <summary>
    ///     Determines whether the specified <see cref="Embed" /> is equal to the current <see cref="Embed" />
    /// </summary>
    /// <param name="embed">The <see cref="Embed" /> to compare with the current <see cref="Embed" /></param>
    /// <returns></returns>
    public bool Equals(Embed? embed)
        => GetHashCode() == embed?.GetHashCode();

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            var hash = 17;
            hash = hash * 23 + (Type, Title, Description, Timestamp, Color, Image, Video, Author, Footer, Provider,
                Thumbnail).GetHashCode();
            foreach (var field in Fields)
                hash = hash * 23 + field.GetHashCode();
            return hash;
        }
    }
}
