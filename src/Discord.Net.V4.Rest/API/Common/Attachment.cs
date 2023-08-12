using System.Text.Json.Serialization;

namespace Discord.API;

internal class Attachment
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("filename")]
    public string Filename { get; set; }

    [JsonPropertyName("description")]
    public Optional<string> Description { get; set; }

    [JsonPropertyName("content_type")]
    public Optional<string> ContentType { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("proxy_url")]
    public string ProxyUrl { get; set; }

    [JsonPropertyName("height")]
    public Optional<int?> Height { get; set; }

    [JsonPropertyName("width")]
    public Optional<int?> Width { get; set; }

    [JsonPropertyName("ephemeral")]
    public Optional<bool> Ephemeral { get; set; }

    [JsonPropertyName("duration_secs")]
    public Optional<double> DurationSeconds { get; set; }

    [JsonPropertyName("waveform")]
    public Optional<string> Waveform { get; set; }

    [JsonPropertyName("flags")]
    public Optional<AttachmentFlags> Flags { get; set; }
}
