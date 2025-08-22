using Discord.Models.Models;

namespace Discord.Models;

public interface IGuildChannelTrait :
    IActor<Snowflake, IGuildChannel>,
    IChannelActor,
    IDeletable,
    IModifiable<IModifyGuildChannelParams, IGuildChannel>
{
    IGuildActor Guild { get; }
    IOverwritesLink Permissions { get; }
}