using Newtonsoft.Json;
using System;

namespace Discord.API.Client
{
    public class Message : MessageReference
    {
        public sealed class Attachment
        {
            [JsonProperty("id")]
            public string Id { get; set; }
            [JsonProperty("url")]
            public string Url { get; set; }
            [JsonProperty("proxy_url")]
            public string ProxyUrl { get; set; }
            [JsonProperty("size")]
            public int Size { get; set; }
            [JsonProperty("filename")]
            public string Filename { get; set; }
            [JsonProperty("width")]
            public int Width { get; set; }
            [JsonProperty("height")]
            public int Height { get; set; }
        }

        public sealed class Embed
        {
            public sealed class Reference
            {
                [JsonProperty("url")]
                public string Url { get; set; }
                [JsonProperty("name")]
                public string Name { get; set; }
            }

            public sealed class ThumbnailInfo
            {
                [JsonProperty("url")]
                public string Url { get; set; }
                [JsonProperty("proxy_url")]
                public string ProxyUrl { get; set; }
                [JsonProperty("width")]
                public int Width { get; set; }
                [JsonProperty("height")]
                public int Height { get; set; }
            }

            [JsonProperty("url")]
            public string Url { get; set; }
            [JsonProperty("type")]
            public string Type { get; set; }
            [JsonProperty("title")]
            public string Title { get; set; }
            [JsonProperty("description")]
            public string Description { get; set; }
            [JsonProperty("author")]
            public Reference Author { get; set; }
            [JsonProperty("provider")]
            public Reference Provider { get; set; }
            [JsonProperty("thumbnail")]
            public ThumbnailInfo Thumbnail { get; set; }
        }

        [JsonProperty("tts")]
        public bool? IsTextToSpeech { get; set; }
        [JsonProperty("mention_everyone")]
        public bool? IsMentioningEveryone { get; set; }
        [JsonProperty("timestamp")]
        public DateTime? Timestamp { get; set; }
        [JsonProperty("edited_timestamp")]
        public DateTime? EditedTimestamp { get; set; }
        [JsonProperty("mentions")]
        public UserReference[] Mentions { get; set; }
        [JsonProperty("embeds")]
        public Embed[] Embeds { get; set; } //TODO: Parse this
        [JsonProperty("attachments")]
        public Attachment[] Attachments { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("author")]
        public UserReference Author { get; set; }
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
    }
}
