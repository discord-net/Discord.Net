namespace Discord.Models;

public interface INewsChannelActor :
    IActor<Snowflake, INewsChannel>,
    IGuildChannelTrait,
    IInvitableGuildChannelTrait
{
}