namespace Discord;

public enum ApplicationCommandContextType
{
    /// <summary>
    ///     The command can be used in guilds.
    /// </summary>
    Guild = 0,

    /// <summary>
    ///     The command can be used in DM channel with the bot.
    /// </summary>
    BotDm = 1,

    /// <summary>
    ///     The command can be used in private channels.
    /// </summary>
    PrivateChannel = 2
}
