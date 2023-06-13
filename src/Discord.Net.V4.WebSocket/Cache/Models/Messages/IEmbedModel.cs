using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public interface IEmbedModel
    {
        string? Title { get; }
        EmbedType? Type { get; }
        string? Description { get; }
        string? Url { get; }
        DateTimeOffset? Timestamp { get; }
        int? Color { get; }

        // footer
        string? FooterText { get; }
        string? FooterIconUrl { get; }
        string? FooterProxyIconUrl { get; }

        // image
        string? ImageUrl { get; }
        string? ImageProxyUrl { get; }
        int? ImageHeight { get; }
        int? ImageWidth { get; }

        // thumbnail
        string? ThumbnailUrl { get; }
        string? ThumbnailProxyUrl { get; }
        int? ThumbnailHeight { get; }
        int? ThumbnailWidth { get; }

        // video
        string? VideoUrl { get; }
        string? VideoProxyUrl { get; }
        int? VideoHeight { get; }
        int? VideoWidth { get; }

        // provider
        string? ProviderName { get; }
        string? ProviderUrl { get; }

        string? AuthorName { get; }
        string? AuthorUrl { get; }
        string? AuthorIconUrl { get; }
        string? AuthorProxyIconUrl { get; }

        IEnumerable<IEmbedFieldModel> Fields { get; }
    }

    public interface IEmbedFieldModel
    {
        string Name { get; }
        string Value { get; }
        bool Inline { get; }
    }
}
