namespace Discord.Models;

public interface IVoiceChannelActor :
    IActor<Snowflake, IVoiceChannel>,
    IChannel,
    IGuildChannelTrait, 
    IMessageChannelTrait,
    INestedChannelTrait,
    IInvitableGuildChannelTrait
{
}