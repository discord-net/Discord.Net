using Discord.Models;

namespace Discord;

public interface IMediaChannelActor :
    IActor<Snowflake, IMediaChannel>,
    IGuildChannelTrait,
    IInvitableGuildChannelTrait
{
    
}