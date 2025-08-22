using Discord.Models.Models;

namespace Discord.Models;

public interface IChannel :
    IEntity<Snowflake>,
    IModeledBy<IChannelModel>
{
    
}