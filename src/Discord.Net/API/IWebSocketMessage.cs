namespace Discord.API
{
    public interface IWebSocketMessage
    {
        int OpCode { get; }
        object Payload { get; }
        bool IsPrivate { get; }
    }
}
