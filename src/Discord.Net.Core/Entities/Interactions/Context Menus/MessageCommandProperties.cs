namespace Discord
{
    /// <summary>
    ///     A class used to create message commands.
    /// </summary>
    public class MessageCommandProperties : ApplicationCommandProperties
    {
        internal override ApplicationCommandType Type => ApplicationCommandType.Message;
    }
}
