using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IEmbedModel
    {
        string Title { get; set; }
        EmbedType Type { get; set; }
        string Description { get; set; }
        string Url { get; set; }
        long? Timestamp { get; set; }
        uint? Color { get; set; }
        string FooterText { get; set; }
        string FooterIconUrl { get; set; }
        string FooterProxyUrl { get; set; }
        string ProviderName { get; set; }
        string ProviderUrl { get; set; }
        string AuthorName { get; set; }
        string AuthorUrl { get; set; }
        string AuthorIconUrl { get; set; }
        string AuthorProxyIconUrl { get; set; }
        IEmbedMediaModel Image { get; set; }
        IEmbedMediaModel Thumbnail { get; set; }
        IEmbedMediaModel Video { get; set; }
        IEmbedFieldModel[] Fields { get; set; }
    }
    public interface IEmbedMediaModel
    {
        string Url { get; set; }
        string ProxyUrl { get; set; }
        int? Height { get; set; }
        int? Width { get; set; }
    }
    public interface IEmbedFieldModel
    {
        string Name { get; set; }
        string Value { get; set; }
        bool Inline { get; set; }
    }
}
