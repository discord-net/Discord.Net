namespace Discord;

public interface IGuildMessageActor : 
    IActor<Snowflake, IGuildMessage>,
    IMessageActor
{
    
}