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

    uint? IEmbedModel.Color => ~Color;
    string? IEmbedModel.Type => ~Type;

    string? IEmbedModel.Description => ~Description;
    string? IEmbedModel.Url => ~Url;

    string? IEmbedModel.Title => ~Title;

    IEnumerable<IEmbedFieldModel> IEmbedModel.Fields => Fields | [];
    IEmbedFooterModel? IEmbedModel.Footer => ~Footer;

    IEmbedImageModel? IEmbedModel.Image => ~Image;

    IEmbedThumbnailModel? IEmbedModel.Thumbnail => ~Thumbnail;

    IEmbedVideoModel? IEmbedModel.Video => ~Video;

    IEmbedProviderModel? IEmbedModel.Provider => ~Provider;

    IEmbedAuthorModel? IEmbedModel.Author => ~Author;
}
