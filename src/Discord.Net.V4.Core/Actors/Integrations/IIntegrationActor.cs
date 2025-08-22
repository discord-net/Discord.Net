namespace Discord.Models;

public interface IIntegrationActor :
    IActor<Snowflake, IIntegration>,
    IDeletable
{
    
}