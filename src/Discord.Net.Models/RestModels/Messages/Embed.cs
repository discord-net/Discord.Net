using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Embed : IEmbedModel
{
    [JsonPropertyName("title")]
    public Optional<string> Title { get; set; }

    [JsonPropertyName("type")]
    public Optional<string> Type { get; set; }

    [JsonPropertyName("description")]
    public Optional<string> Description { get; set; }

    [JsonPropertyName("url")]
    public Optional<string> Url { get; set; }

    [JsonPropertyName("color")]
    public Optional<uint> Color { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTimeOffset? Timestamp { get; set; }

    [JsonPropertyName("author")]
    public Optional<EmbedAuthor> Author { get; set; }

    [JsonPropertyName("footer")]
    public Optional<EmbedFooter> Footer { get; set; }

    [JsonPropertyName("video")]
    public Optional<EmbedVideo> Video { get; set; }

    [JsonPropertyName("thumbnail")]
    public Optional<EmbedThumbnail> Thumbnail { get; set; }

    [JsonPropertyName("image")]
    public Optional<EmbedImage> Image { get; set; }

    [JsonPropertyName("provider")]
    public Optional<EmbedProvider> Provider { get; set; }

    [JsonPropertyName("fields")]
    public Optional<EmbedField[]> Fields { get; set; }

    uint? IEmbedModel.Color => Color;
    string? IEmbedModel.Type => Type;

    string? IEmbedModel.Description => Description;
    string? IEmbedModel.Url => Url;

    string? IEmbedModel.Title => Title;

    string? IEmbedModel.FooterText => Footer.Map(v => v.Text);

    string? IEmbedModel.FooterIconUrl => Footer.Map(v => v.IconUrl);

    string? IEmbedModel.FooterProxyIconUrl => Footer.Map(v => v.ProxyIconUrl);

    string? IEmbedModel.ImageUrl => Image.Map(v => v.Url);

    string? IEmbedModel.ImageProxyUrl => Image.Map(v => v.ProxyUrl);

    int? IEmbedModel.ImageHeight => Image.Map(v => v.Height);

    int? IEmbedModel.ImageWidth => Image.Map(v => v.Width);

    string? IEmbedModel.ThumbnailUrl => Thumbnail.Map(v => v.Url);

    string? IEmbedModel.ThumbnailProxyUrl => Thumbnail.Map(v => v.ProxyUrl);

    int? IEmbedModel.ThumbnailHeight => Thumbnail.Map(v => v.Height);

    int? IEmbedModel.ThumbnailWidth => Thumbnail.Map(v => v.Width);

    string? IEmbedModel.VideoUrl => Video.Map(v => v.Url);

    string? IEmbedModel.VideoProxyUrl => Video.Map(v => v.ProxyUrl);

    int? IEmbedModel.VideoHeight => Video.Map(v => v.Height);

    int? IEmbedModel.VideoWidth => Video.Map(v => v.Width);

    string? IEmbedModel.ProviderName => Provider.Map(v => v.Name);

    string? IEmbedModel.ProviderUrl => Provider.Map(v => v.Url);

    string? IEmbedModel.AuthorName => Author.Map(v => v.Name);

    string? IEmbedModel.AuthorUrl => Author.Map(v => v.Url);

    string? IEmbedModel.AuthorIconUrl => Author.Map(v => v.IconUrl);

    string? IEmbedModel.AuthorProxyIconUrl => Author.Map(v => v.ProxyIconUrl);

    IEnumerable<IEmbedFieldModel> IEmbedModel.Fields => Fields | [];
}
