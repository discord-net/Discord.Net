using System;
using Newtonsoft.Json;
using Discord.Net.Converters;

namespace Discord.API
{
    internal class Embed : IEmbedModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("color")]
        public uint? Color { get; set; }
        [JsonProperty("type"), JsonConverter(typeof(EmbedTypeConverter))]
        public EmbedType Type { get; set; }
        [JsonProperty("timestamp")]
        public DateTimeOffset? Timestamp { get; set; }
        [JsonProperty("author")]
        public Optional<EmbedAuthor> Author { get; set; }
        [JsonProperty("footer")]
        public Optional<EmbedFooter> Footer { get; set; }
        [JsonProperty("video")]
        public Optional<EmbedVideo> Video { get; set; }
        [JsonProperty("thumbnail")]
        public Optional<EmbedThumbnail> Thumbnail { get; set; }
        [JsonProperty("image")]
        public Optional<EmbedImage> Image { get; set; }
        [JsonProperty("provider")]
        public Optional<EmbedProvider> Provider { get; set; }
        [JsonProperty("fields")]
        public Optional<EmbedField[]> Fields { get; set; }

        EmbedType IEmbedModel.Type { get => Type; set => throw new NotSupportedException(); }
        DateTimeOffset? IEmbedModel.Timestamp { get => Timestamp; set => throw new NotSupportedException(); }
        IEmbedFooterModel IEmbedModel.Footer { get => Footer.GetValueOrDefault(); set => throw new NotSupportedException(); }
        IEmbedMediaModel IEmbedModel.Image { get => Image.GetValueOrDefault(); set => throw new NotSupportedException(); }
        IEmbedMediaModel IEmbedModel.Thumbnail { get => Thumbnail.GetValueOrDefault(); set => throw new NotSupportedException(); }
        IEmbedMediaModel IEmbedModel.Video { get => Video.GetValueOrDefault(); set => throw new NotSupportedException(); }
        IEmbedProviderModel IEmbedModel.Provider { get => Provider.GetValueOrDefault(); set => throw new NotSupportedException(); }
        IEmbedAuthorModel IEmbedModel.Author { get => Author.GetValueOrDefault(); set => throw new NotSupportedException(); }
        IEmbedFieldModel[] IEmbedModel.Fields { get => Fields.GetValueOrDefault(); set => throw new NotSupportedException(); }
    }
}
