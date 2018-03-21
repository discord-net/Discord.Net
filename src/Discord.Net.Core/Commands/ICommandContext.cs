namespace Discord.Commands
{
    public interface ICommandContext
    {
        /// <summary> The Discord client of which the command is executed with. </summary>
        IDiscordClient Client { get; }
        /// <summary> The guild of which the command is executed in. </summary>
        IGuild Guild { get; }
        /// <summary> The channel of which the command is executed in. </summary>
        IMessageChannel Channel { get; }
        /// <summary> The user who executed the command. </summary>
        IUser User { get; }
        /// <summary> The message of which the command is interpreted from. </summary>
        IUserMessage Message { get; }
    }
}
