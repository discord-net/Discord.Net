using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Attachment : IAttachmentModel
{


    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("filename")]
    public required string Filename { get; set; }

    [JsonPropertyName("description")]
    public Optional<string> Description { get; set; }

    [JsonPropertyName("content_type")]
    public Optional<string> ContentType { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("url")]
    public required string Url { get; set; }

    [JsonPropertyName("proxy_url")]
    public required string ProxyUrl { get; set; }

    [JsonPropertyName("height")]
    public Optional<int?> Height { get; set; }

    [JsonPropertyName("width")]
    public Optional<int?> Width { get; set; }

    [JsonPropertyName("ephemeral")]
    public Optional<bool> Ephemeral { get; set; }

    [JsonPropertyName("duration_secs")]
    public Optional<float> DurationSeconds { get; set; }

    [JsonPropertyName("waveform")]
    public Optional<string> Waveform { get; set; }

    [JsonPropertyName("flags")]
    public Optional<int> Flags { get; set; }

    int? IAttachmentModel.Height => Height;

    int? IAttachmentModel.Width => Width;

    bool IAttachmentModel.Ephemeral => Ephemeral;

    string? IAttachmentModel.Description => Description;

    string? IAttachmentModel.ContentType => ContentType;

    float? IAttachmentModel.Duration => DurationSeconds;

    string? IAttachmentModel.Waveform => Waveform;
}
