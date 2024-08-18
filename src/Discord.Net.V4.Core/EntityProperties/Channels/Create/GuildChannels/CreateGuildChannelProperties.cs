using Discord.Models.Json;

namespace Discord;

public class CreateGuildChannelProperties : CreateGuildChannelBaseProperties
{
    public Optional<ChannelType> Type { get; set; }

    protected override Optional<ChannelType> ChannelType => Type;
}
