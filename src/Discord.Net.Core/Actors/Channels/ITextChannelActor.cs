using Discord.Models;

namespace Discord;

public interface ITextChannelActor :
    IActor<Snowflake, ITextChannel>,
    IChannelActor,
    IGuildChannelTrait, 
    INestedChannelTrait,
    IMessageChannelTrait,
    IInvitableGuildChannelTrait,
    IModifiable<IModifyTextChannelParams, ITextChannel>;