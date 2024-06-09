using Discord.Models;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class WelcomeScreen : IWelcomeScreenModel
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("welcome_channels")]
    public required WelcomeScreenChannel[] WelcomeChannels { get; set; }

    IWelcomeScreenChannelModel[] IWelcomeScreenModel.WelcomeChannels => WelcomeChannels;
}
