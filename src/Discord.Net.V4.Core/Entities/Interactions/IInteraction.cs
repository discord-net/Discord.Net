using Discord.Models;

namespace Discord;

public partial interface IInteraction :
    ISnowflakeEntity<IInteractionModel>,
    IInteractionActor
{
    
}