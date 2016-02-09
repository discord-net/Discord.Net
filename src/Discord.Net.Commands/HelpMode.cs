namespace Discord.Commands
{
    public enum HelpMode
    {
        /// <summary> Disable the automatic help command. </summary>
        Disabled,
        /// <summary> Use the automatic help command and respond in the channel the command is used. </summary>
        Public,
        /// <summary> Use the automatic help command and respond in a private message. </summary>
        Private
    }
}
