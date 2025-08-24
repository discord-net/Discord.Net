namespace Discord;

public interface IPinnedMessageActor : 
    IActor<Snowflake, IPinnedMessage>,
    IDeletable
{
    
}