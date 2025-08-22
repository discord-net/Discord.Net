using Discord.Models;

namespace Discord;

public interface ICategoryChannelActor :
    IActor<Snowflake, ICategoryChannel>,
    IChannelActor,
    IGuildChannelTrait;