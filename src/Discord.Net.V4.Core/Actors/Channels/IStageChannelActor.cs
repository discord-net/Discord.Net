namespace Discord.Models;

public interface IStageChannelActor :
    IActor<Snowflake, IStageChannel>,
    IGuildChannelTrait,
    IInvitableGuildChannelTrait
{
    
}