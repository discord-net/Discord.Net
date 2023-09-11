using Discord.Models;
using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class WelcomeScreen : IWelcomeScreenModel
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("welcome_channels")]
    public WelcomeScreenChannel[] WelcomeChannels { get; set; }

    IWelcomeScreenChannelModel[] IWelcomeScreenModel.WelcomeChannels => WelcomeChannels;
}
