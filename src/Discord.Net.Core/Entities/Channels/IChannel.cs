using Discord.Models;

namespace Discord;

public interface IChannel :
    IEntity<Snowflake>,
    IModeledBy<IChannelModel>
{
    
}