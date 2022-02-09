namespace Discord
{
    /// <summary>
    ///     The option type of the Slash command parameter, See <see href="https://discord.com/developers/docs/interactions/slash-commands#applicationcommandoptiontype">the discord docs</see>.
    /// </summary>
    public enum ApplicationCommandOptionType : byte
    {
        /// <summary>
        ///     A sub command.
        /// </summary>
        SubCommand = 1,

        /// <summary>
        ///     A group of sub commands.
        /// </summary>
        SubCommandGroup = 2,

        /// <summary>
        ///     A <see langword="string"/> of text.
        /// </summary>
        String = 3,

        /// <summary>
        ///     An <see langword="int"/>.
        /// </summary>
        Integer = 4,

        /// <summary>
        ///     A <see langword="bool"/>.
        /// </summary>
        Boolean = 5,

        /// <summary>
        ///     A <see cref="IUser"/>.
        /// </summary>
        User = 6,

        /// <summary>
        ///     A <see cref="IGuildChannel"/>.
        /// </summary>
        Channel = 7,

        /// <summary>
        ///     A <see cref="IRole"/>.
        /// </summary>
        Role = 8,

        /// <summary>
        ///     A <see cref="IUser"/> or <see cref="IRole"/>.
        /// </summary>
        Mentionable = 9,

        /// <summary>
        ///     A <see cref="double"/>.
        /// </summary>
        Number = 10,

        /// <summary>
        ///     A <see cref="Discord.Attachment"/>.
        /// </summary>
        Attachment = 11
    }
}
