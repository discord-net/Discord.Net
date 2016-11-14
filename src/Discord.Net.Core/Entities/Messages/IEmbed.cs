using System.Collections.Immutable;

namespace Discord
{
    public interface IEmbed
    {
        string Url { get; }
        string Type { get; }
        string Title { get; }
        string Description { get; }
        Color? Color { get; }
        EmbedAuthor? Author { get; }
        EmbedFooter? Footer { get; }
        EmbedProvider? Provider { get; }
        EmbedThumbnail? Thumbnail { get; }
        ImmutableArray<EmbedField> Fields { get; }
    }
}
