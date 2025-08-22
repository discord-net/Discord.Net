using Discord.Models;
using Discord.Models.Models;

namespace Discord;

public interface IChannel :
    IEntity<Snowflake>,
    IModeledBy<IChannelModel>
{
    
}