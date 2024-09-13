namespace Discord.Models;

[ModelEquality]
public partial interface IChannelApplicationCommandOptionModel : IApplicationCommandOptionModel
{
    bool? IsRequired { get; }
    int[]? ChannelTypes { get; }
}