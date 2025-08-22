namespace Discord.Models;

public interface ICategoryChannelActor :
    IActor<Snowflake, ICategoryChannel>,
    IChannelActor,
    IGuildChannelTrait;