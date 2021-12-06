namespace Discord.Rest
{
    /// <summary>
    ///     Represents a Rest based context of an <see cref="IDiscordInteraction"/>.
    /// </summary>
    public class RestInteractionContext<TInteraction> : IInteractionContext
        where TInteraction : RestInteraction
    {
        /// <summary>
        ///     Gets the <see cref="DiscordRestClient"/> that the command will be executed with.
        /// </summary>
        public DiscordRestClient Client { get; }

        /// <summary>
        ///     Gets the <see cref="RestGuild"/> the command originated from.
        /// </summary>
        /// <remarks>
        ///     Will be null if the command is from a DM Channel.
        /// </remarks>
        public RestGuild Guild { get; }

        /// <summary>
        ///     Gets the <see cref="IRestMessageChannel"/> the command originated from.
        /// </summary>
        public IRestMessageChannel Channel { get; }

        /// <summary>
        ///     Gets the <see cref="RestUser"/> who executed the command.
        /// </summary>
        public RestUser User { get; }

        /// <summary>
        ///     Gets the <see cref="RestInteraction"/> the command was recieved with.
        /// </summary>
        public TInteraction Interaction { get; }

        /// <summary>
        ///     Initializes a new <see cref="RestInteractionContext{TInteraction}"/>. 
        /// </summary>
        /// <param name="client">The underlying client.</param>
        /// <param name="interaction">The underlying interaction.</param>
        public RestInteractionContext(DiscordRestClient client, TInteraction interaction)
        {
            Client = client;
            Guild = interaction.Guild;
            Channel = interaction.Channel;
            User = interaction.User;
            Interaction = interaction;
        }

        // IInterationContext
        /// <inheritdoc/>
        IDiscordClient IInteractionContext.Client => Client;

        /// <inheritdoc/>
        IGuild IInteractionContext.Guild => Guild;

        /// <inheritdoc/>
        IMessageChannel IInteractionContext.Channel => Channel;

        /// <inheritdoc/>
        IUser IInteractionContext.User => User;

        /// <inheritdoc/>
        IDiscordInteraction IInteractionContext.Interaction => Interaction;
    }

    /// <summary>
    ///     Represents a Rest based context of an <see cref="IDiscordInteraction"/>
    /// </summary>
    public class RestInteractionContext : RestInteractionContext<RestInteraction>
    {
        /// <summary>
        ///     Initializes a new <see cref="RestInteractionContext"/> 
        /// </summary>
        /// <param name="client">The underlying client</param>
        /// <param name="interaction">The underlying interaction</param>
        public RestInteractionContext(DiscordRestClient client, RestInteraction interaction) : base(client, interaction) { }
    }
}
