namespace Discord.Models;

public interface IMediaChannelActor :
    IActor<Snowflake, IMediaChannel>,
    IGuildChannelTrait,
    IInvitableGuildChannelTrait
{
    
}