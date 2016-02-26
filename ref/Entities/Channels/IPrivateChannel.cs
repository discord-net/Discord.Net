namespace Discord
{
    public interface IPrivateChannel : IChannel
    {
        /// <summary> Gets the recipient of the messages in this private channel. </summary>
        User Recipient { get; }
    }
}
