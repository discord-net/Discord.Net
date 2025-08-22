namespace Discord.Models;

public interface IDMChannel : 
    IChannel
{
    IMessagesLink Messages { get; }
}