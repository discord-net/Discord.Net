namespace Discord
{
    public interface ISystemMessage : IMessage
    {
        /// <summary> Gets the type of this system message. </summary>
        MessageType Type { get; }
    }
}
