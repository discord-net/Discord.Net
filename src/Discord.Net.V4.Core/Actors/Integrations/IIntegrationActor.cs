using Discord.Models;

namespace Discord;

public interface IIntegrationActor :
    IActor<Snowflake, IIntegration>,
    IDeletable
{
    
}