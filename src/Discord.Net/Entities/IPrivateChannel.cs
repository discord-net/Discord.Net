namespace Discord
{
    public interface IPrivateChannel : IChannel
    {
        User Recipient { get; }
    }
}
