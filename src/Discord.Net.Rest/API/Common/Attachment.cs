using Newtonsoft.Json;

namespace Discord.API;

internal class Attachment
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("filename")]
    public string Filename { get; set; }

    [JsonProperty("description")]
    public Optional<string> Description { get; set; }

    [JsonProperty("content_type")]
    public Optional<string> ContentType { get; set; }

    [JsonProperty("size")]
    public int Size { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("proxy_url")]
    public string ProxyUrl { get; set; }

    [JsonProperty("height")]
    public Optional<int> Height { get; set; }

    [JsonProperty("width")]
    public Optional<int> Width { get; set; }

    [JsonProperty("ephemeral")]
    public Optional<bool> Ephemeral { get; set; }

    [JsonProperty("duration_secs")]
    public Optional<double> DurationSeconds { get; set; }

    [JsonProperty("waveform")]
    public Optional<string> Waveform { get; set; }

    [JsonProperty("flags")]
    public Optional<AttachmentFlags> Flags { get; set; }
}
