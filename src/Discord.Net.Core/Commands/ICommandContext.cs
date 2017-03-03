namespace Discord.Commands
{
    public interface ICommandContext
    {
        IDiscordClient Client { get; }
        IGuild Guild { get; }
        IMessageChannel Channel { get; }
        IUser User { get; }
        IUserMessage Message { get; }
    }
}
