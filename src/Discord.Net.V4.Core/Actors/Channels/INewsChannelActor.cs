using Discord.Models;

namespace Discord;

public interface INewsChannelActor :
    IActor<Snowflake, INewsChannel>,
    IGuildChannelTrait,
    IInvitableGuildChannelTrait
{
}