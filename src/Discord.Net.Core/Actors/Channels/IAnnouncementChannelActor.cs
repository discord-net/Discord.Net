using Discord.Models;

namespace Discord;

public interface IAnnouncementChannelActor :
    IActor<Snowflake, IAnnouncementChannel>,
    IGuildChannelTrait,
    IGuildMessageChannelTrait,
    IInvitableGuildChannelTrait
{
}