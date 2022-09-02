using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.Interactions
{
    /// <inheritdoc cref="IInteractionContext"/>
    public class InteractionContext : IInteractionContext, IRouteMatchContainer
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
        /// <inheritdoc cref="IRouteMatchContainer.SegmentMatches"/>
        public IReadOnlyCollection<IRouteSegmentMatch> SegmentMatches { get; private set; }

        /// <summary>
        ///     Initializes a new <see cref="SocketInteractionContext{TInteraction}"/>.
        /// </summary>
        /// <param name="client">The underlying client.</param>
        /// <param name="interaction">The underlying interaction.</param>
        /// <param name="channel"><see cref="IMessageChannel"/> the command originated from.</param>
        public InteractionContext(IDiscordClient client, IDiscordInteraction interaction, IMessageChannel channel = null)
        {
            Client = client;
            Interaction = interaction;
            Channel = channel;
            Guild = (interaction.User as IGuildUser)?.Guild;
            User = interaction.User;
            Interaction = interaction;
        }

        /// <inheritdoc/>
        public void SetSegmentMatches(IEnumerable<IRouteSegmentMatch> segmentMatches) => SegmentMatches = segmentMatches.ToImmutableArray();

        //IRouteMatchContainer
        /// <inheritdoc/>
        IEnumerable<IRouteSegmentMatch> IRouteMatchContainer.SegmentMatches => SegmentMatches;
    }
}
