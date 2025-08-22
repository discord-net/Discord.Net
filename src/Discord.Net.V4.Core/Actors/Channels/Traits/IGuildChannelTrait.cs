using Discord.Models;

namespace Discord;

public interface IGuildChannelTrait :
    IActor<Snowflake, IGuildChannel>,
    IChannelActor,
    IDeletable,
    IModifiable<IModifyGuildChannelParams, IGuildChannel>
{
    IGuildActor Guild { get; }
    IOverwritesLink Permissions { get; }
}