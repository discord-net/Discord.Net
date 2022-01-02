namespace Discord.Interactions
{
    /// <inheritdoc cref="IInteractionContext"/>
    public class InteractionContext : IInteractionContext
    {
        /// <inheritdoc/>
        public IDiscordClient Client { get; }
        /// <inheritdoc/>
        public IGuild Guild { get; }
        /// <inheritdoc/>
        public IMessageChannel Channel { get; }
        /// <inheritdoc/>
        public IUser User { get; }
        /// <inheritdoc/>
        public IDiscordInteraction Interaction { get; }

        /// <summary>
        ///     Initializes a new <see cref="SocketInteractionContext{TInteraction}"/>.
        /// </summary>
        /// <param name="client">The underlying client.</param>
        /// <param name="interaction">The underlying interaction.</param>
        /// <param name="user"><see cref="IUser"/> who executed the command.</param>
        /// <param name="channel"><see cref="ISocketMessageChannel"/> the command originated from.</param>
        public InteractionContext(IDiscordClient client, IDiscordInteraction interaction, IMessageChannel channel = null)
        {
            Client = client;
            Interaction = interaction;
            Channel = channel;
            Guild = (interaction.User as IGuildUser)?.Guild;
            User = interaction.User;
            Interaction = interaction;
        }
    }
}
