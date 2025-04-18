using Newtonsoft.Json;

namespace Discord.API;

internal class UnfurledMediaItem
{
    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("proxy_url")]
    public Optional<string> ProxyUrl { get; set; }

    [JsonProperty("height")]
    public Optional<int?> Height { get; set; }

    [JsonProperty("width")]
    public Optional<int?> Width { get; set; }

    [JsonProperty("content_type")]
    public Optional<string> ContentType { get; set; }

    [JsonProperty("loading_state")]
    public Optional<UnfurledMediaItemLoadingState> LoadingState { get; set; }
}
