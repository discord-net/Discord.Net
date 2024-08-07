using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ClientStatus : IClientStatusModel
{
    [JsonPropertyName("desktop")]
    public Optional<string> Desktop { get; set; }

    [JsonPropertyName("mobile")]
    public Optional<string> Mobile { get; set; }

    [JsonPropertyName("web")]
    public Optional<string> Web { get; set; }

    string? IClientStatusModel.Mobile => ~Mobile;

    string? IClientStatusModel.Web => ~Web;

    string? IClientStatusModel.Desktop => ~Desktop;
}
