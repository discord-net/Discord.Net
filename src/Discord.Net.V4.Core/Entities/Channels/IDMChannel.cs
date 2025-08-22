namespace Discord;

public interface IDMChannel : 
    IChannel
{
    IMessagesLink Messages { get; }
}