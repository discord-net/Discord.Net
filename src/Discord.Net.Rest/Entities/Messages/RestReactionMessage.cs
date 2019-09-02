using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Message;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based mesage that reactions can be added to.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public abstract class RestReactionMessage : RestMessage, IReactionMessage
    {
        private ImmutableArray<RestReaction> _reactions = ImmutableArray.Create<RestReaction>();

        /// <inheritdoc />
        public IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions => _reactions.ToDictionary(x => x.Emote, x => new ReactionMetadata { ReactionCount = x.Count, IsMe = x.Me });

        internal RestReactionMessage(BaseDiscordClient discord, ulong id, IMessageChannel channel, IUser author, MessageSource source)
            : base(discord, id, channel, author, source)
        {
        }

        internal override void Update(Model model)
        {
            base.Update(model);

            if (model.Reactions.IsSpecified)
            {
                var value = model.Reactions.Value;
                if (value.Length > 0)
                {
                    var reactions = ImmutableArray.CreateBuilder<RestReaction>(value.Length);
                    for (int i = 0; i < value.Length; i++)
                        reactions.Add(RestReaction.Create(value[i]));
                    _reactions = reactions.ToImmutable();
                }
                else
                    _reactions = ImmutableArray.Create<RestReaction>();
            }
            else
                _reactions = ImmutableArray.Create<RestReaction>();
        }

        /// <inheritdoc />
        public Task AddReactionAsync(IEmote emote, RequestOptions options = null)
            => MessageHelper.AddReactionAsync(this, emote, Discord, options);
        /// <inheritdoc />
        public Task RemoveReactionAsync(IEmote emote, IUser user, RequestOptions options = null)
            => MessageHelper.RemoveReactionAsync(this, user.Id, emote, Discord, options);
        /// <inheritdoc />
        public Task RemoveReactionAsync(IEmote emote, ulong userId, RequestOptions options = null)
            => MessageHelper.RemoveReactionAsync(this, userId, emote, Discord, options);
        /// <inheritdoc />
        public Task RemoveAllReactionsAsync(RequestOptions options = null)
            => MessageHelper.RemoveAllReactionsAsync(this, Discord, options);
        /// <inheritdoc />
        public IAsyncEnumerable<IReadOnlyCollection<IUser>> GetReactionUsersAsync(IEmote emote, int limit, RequestOptions options = null)
            => MessageHelper.GetReactionUsersAsync(this, emote, limit, Discord, options);
    }
}
