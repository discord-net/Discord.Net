using Discord.Models.Models;

namespace Discord.Models;

public interface ITextChannelActor :
    IActor<Snowflake, ITextChannel>,
    IChannelActor,
    IGuildChannelTrait, 
    INestedChannelTrait,
    IMessageChannelTrait,
    IInvitableGuildChannelTrait,
    IModifiable<IModifyTextChannelParams, ITextChannel>;