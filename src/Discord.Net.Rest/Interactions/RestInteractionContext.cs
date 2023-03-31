using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a Rest based context of an <see cref="IDiscordInteraction"/>.
    /// </summary>
    public class RestInteractionContext<TInteraction> : IRestInteractionContext, IRouteMatchContainer
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
        ///     Gets the <see cref="RestInteraction"/> the command was received with.
        /// </summary>
        public TInteraction Interaction { get; }

        /// <summary>
        ///     Gets or sets the callback to use when the service has outgoing json for the rest webhook.
        /// </summary>
        /// <remarks>
        ///     If this property is <see langword="null"/> the default callback will be used.
        /// </remarks>
        public Func<string, Task> InteractionResponseCallback { get; set; }

        /// <inheritdoc cref="IRouteMatchContainer.SegmentMatches"/>
        public IReadOnlyCollection<IRouteSegmentMatch> SegmentMatches { get; private set; }

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

        /// <summary>
        ///     Initializes a new <see cref="RestInteractionContext{TInteraction}"/>.
        /// </summary>
        /// <param name="client">The underlying client.</param>
        /// <param name="interaction">The underlying interaction.</param>
        /// <param name="interactionResponseCallback">The callback for outgoing json.</param>
        public RestInteractionContext(DiscordRestClient client, TInteraction interaction, Func<string, Task> interactionResponseCallback)
            : this(client, interaction)
        {
            InteractionResponseCallback = interactionResponseCallback;
        }

        /// <inheritdoc/>
        public void SetSegmentMatches(IEnumerable<IRouteSegmentMatch> segmentMatches) => SegmentMatches = segmentMatches.ToImmutableArray();

        //IRouteMatchContainer
        /// <inheritdoc/>
        IEnumerable<IRouteSegmentMatch> IRouteMatchContainer.SegmentMatches => SegmentMatches;

        // IInteractionContext
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
    ///     Represents a Rest based context of an <see cref="IDiscordInteraction"/>.
    /// </summary>
    public class RestInteractionContext : RestInteractionContext<RestInteraction>
    {
        /// <summary>
        ///     Initializes a new <see cref="RestInteractionContext"/>.
        /// </summary>
        /// <param name="client">The underlying client.</param>
        /// <param name="interaction">The underlying interaction.</param>
        public RestInteractionContext(DiscordRestClient client, RestInteraction interaction) : base(client, interaction) { }

        /// <summary>
        ///     Initializes a new <see cref="RestInteractionContext"/>.
        /// </summary>
        /// <param name="client">The underlying client.</param>
        /// <param name="interaction">The underlying interaction.</param>
        /// <param name="interactionResponseCallback">The callback for outgoing json.</param>
        public RestInteractionContext(DiscordRestClient client, RestInteraction interaction, Func<string, Task> interactionResponseCallback)
            : base(client, interaction, interactionResponseCallback) { }
    }
}
