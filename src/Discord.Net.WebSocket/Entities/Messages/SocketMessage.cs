using Discord.Rest;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Message;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based message.
    /// </summary>
    public abstract class SocketMessage : SocketEntity<ulong>, IMessage
    {
        #region SocketMessage
        private long _timestampTicks;
        private readonly List<SocketReaction> _reactions = new List<SocketReaction>();
        private ImmutableArray<SocketUser> _userMentions = ImmutableArray.Create<SocketUser>();

        /// <summary>
        ///     Gets the author of this message.
        /// </summary>
        /// <returns>
        ///     A WebSocket-based user object.
        /// </returns>
        public SocketUser Author { get; }
        /// <summary>
        ///     Gets the source channel of the message.
        /// </summary>
        /// <returns>
        ///     A WebSocket-based message channel.
        /// </returns>
        public ISocketMessageChannel Channel { get; }
        /// <inheritdoc />
        public MessageSource Source { get; }

        /// <inheritdoc />
        public string Content { get; private set; }

        /// <inheritdoc />
        public string CleanContent => MessageHelper.SanitizeMessage(this);

        /// <inheritdoc />
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);
        /// <inheritdoc />
        public virtual bool IsTTS => false;
        /// <inheritdoc />
        public virtual bool IsPinned => false;
        /// <inheritdoc />
        public virtual bool IsSuppressed => false;
        /// <inheritdoc />
        public virtual DateTimeOffset? EditedTimestamp => null;
        /// <inheritdoc />
        public virtual bool MentionedEveryone => false;

        /// <inheritdoc />
        public MessageActivity Activity { get; private set; }

        /// <inheritdoc />
        public MessageApplication Application { get; private set; }

        /// <inheritdoc />
        public MessageReference Reference { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyCollection<ActionRowComponent> Components { get; private set; }

        /// <summary>
        ///     Gets the interaction this message is a response to.
        /// </summary>
        public MessageInteraction<SocketUser> Interaction { get; private set; }

        /// <inheritdoc />
        public MessageFlags? Flags { get; private set; }

        /// <inheritdoc/>
        public MessageType Type { get; private set; }

        /// <summary>
        ///     Returns all attachments included in this message.
        /// </summary>
        /// <returns>
        ///     Collection of attachments.
        /// </returns>
        public virtual IReadOnlyCollection<Attachment> Attachments => ImmutableArray.Create<Attachment>();
        /// <summary>
        ///     Returns all embeds included in this message.
        /// </summary>
        /// <returns>
        ///     Collection of embed objects.
        /// </returns>
        public virtual IReadOnlyCollection<Embed> Embeds => ImmutableArray.Create<Embed>();
        /// <summary>
        ///     Returns the channels mentioned in this message.
        /// </summary>
        /// <returns>
        ///     Collection of WebSocket-based guild channels.
        /// </returns>
        public virtual IReadOnlyCollection<SocketGuildChannel> MentionedChannels => ImmutableArray.Create<SocketGuildChannel>();
        /// <summary>
        ///     Returns the roles mentioned in this message.
        /// </summary>
        /// <returns>
        ///     Collection of WebSocket-based roles.
        /// </returns>
        public virtual IReadOnlyCollection<SocketRole> MentionedRoles => ImmutableArray.Create<SocketRole>();
        /// <inheritdoc />
        public virtual IReadOnlyCollection<ITag> Tags => ImmutableArray.Create<ITag>();
        /// <inheritdoc />
        public virtual IReadOnlyCollection<SocketSticker> Stickers => ImmutableArray.Create<SocketSticker>();
        /// <inheritdoc />
        public IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions => _reactions.GroupBy(r => r.Emote).ToDictionary(x => x.Key, x => new ReactionMetadata { ReactionCount = x.Count(), IsMe = x.Any(y => y.UserId == Discord.CurrentUser.Id) });
        /// <summary>
        ///     Returns the users mentioned in this message.
        /// </summary>
        /// <returns>
        ///     Collection of WebSocket-based users.
        /// </returns>
        public IReadOnlyCollection<SocketUser> MentionedUsers => _userMentions; 
        /// <inheritdoc />
        public DateTimeOffset Timestamp => DateTimeUtils.FromTicks(_timestampTicks);

        internal SocketMessage(DiscordSocketClient discord, ulong id, ISocketMessageChannel channel, SocketUser author, MessageSource source)
            : base(discord, id)
        {
            Channel = channel;
            Author = author;
            Source = source;
        }
        internal static SocketMessage Create(DiscordSocketClient discord, ClientState state, SocketUser author, ISocketMessageChannel channel, Model model)
        {
            if (model.Type == MessageType.Default ||
                model.Type == MessageType.Reply ||
                model.Type == MessageType.ApplicationCommand ||
                model.Type == MessageType.ThreadStarterMessage ||
                model.Type == MessageType.ContextMenuCommand)
                return SocketUserMessage.Create(discord, state, author, channel, model);
            else
                return SocketSystemMessage.Create(discord, state, author, channel, model);
        }
        internal virtual void Update(ClientState state, Model model)
        {
            Type = model.Type;

            if (model.Timestamp.IsSpecified)
                _timestampTicks = model.Timestamp.Value.UtcTicks;

            if (model.Content.IsSpecified)
            {
                Content = model.Content.Value;
            }

            if (model.Application.IsSpecified)
            {
                // create a new Application from the API model
                Application = new MessageApplication()
                {
                    Id = model.Application.Value.Id,
                    CoverImage = model.Application.Value.CoverImage,
                    Description = model.Application.Value.Description,
                    Icon = model.Application.Value.Icon,
                    Name = model.Application.Value.Name
                };
            }

            if (model.Activity.IsSpecified)
            {
                // create a new Activity from the API model
                Activity = new MessageActivity()
                {
                    Type = model.Activity.Value.Type.Value,
                    PartyId = model.Activity.Value.PartyId.GetValueOrDefault()
                };
            }

            if (model.Reference.IsSpecified)
            {
                // Creates a new Reference from the API model
                Reference = new MessageReference
                {
                    GuildId = model.Reference.Value.GuildId,
                    InternalChannelId = model.Reference.Value.ChannelId,
                    MessageId = model.Reference.Value.MessageId,
                    FailIfNotExists = model.Reference.Value.FailIfNotExists
                };
            }

            if (model.Components.IsSpecified)
            {
                Components = model.Components.Value.Select(x => new ActionRowComponent(x.Components.Select<IMessageComponent, IMessageComponent>(y =>
                {
                    switch (y.Type)
                    {
                        case ComponentType.Button:
                            {
                                var parsed = (API.ButtonComponent)y;
                                return new Discord.ButtonComponent(
                                    parsed.Style,
                                    parsed.Label.GetValueOrDefault(),
                                    parsed.Emote.IsSpecified
                                        ? parsed.Emote.Value.Id.HasValue
                                            ? new Emote(parsed.Emote.Value.Id.Value, parsed.Emote.Value.Name, parsed.Emote.Value.Animated.GetValueOrDefault())
                                            : new Emoji(parsed.Emote.Value.Name)
                                        : null,
                                    parsed.CustomId.GetValueOrDefault(),
                                    parsed.Url.GetValueOrDefault(),
                                    parsed.Disabled.GetValueOrDefault());
                            }
                        case ComponentType.SelectMenu:
                            {
                                var parsed = (API.SelectMenuComponent)y;
                                return new SelectMenuComponent(
                                    parsed.CustomId,
                                    parsed.Options.Select(z => new SelectMenuOption(
                                        z.Label,
                                        z.Value,
                                        z.Description.GetValueOrDefault(),
                                        z.Emoji.IsSpecified
                                        ? z.Emoji.Value.Id.HasValue
                                            ? new Emote(z.Emoji.Value.Id.Value, z.Emoji.Value.Name, z.Emoji.Value.Animated.GetValueOrDefault())
                                            : new Emoji(z.Emoji.Value.Name)
                                        : null,
                                        z.Default.ToNullable())).ToList(),
                                    parsed.Placeholder.GetValueOrDefault(),
                                    parsed.MinValues,
                                    parsed.MaxValues,
                                    parsed.Disabled
                                    );
                            }
                        default:
                            return null;
                    }
                }).ToList())).ToImmutableArray();
            }
            else
                Components = new List<ActionRowComponent>();

            if (model.UserMentions.IsSpecified)
            {
                var value = model.UserMentions.Value;
                if (value.Length > 0)
                {
                    var newMentions = ImmutableArray.CreateBuilder<SocketUser>(value.Length);
                    for (int i = 0; i < value.Length; i++)
                    {
                        var val = value[i];
                        if (val != null)
                        {
                            var user = Channel.GetUserAsync(val.Id, CacheMode.CacheOnly).GetAwaiter().GetResult() as SocketUser;
                            if (user != null)
                                newMentions.Add(user);
                            else
                                newMentions.Add(SocketUnknownUser.Create(Discord, state, val));
                        }
                    }
                    _userMentions = newMentions.ToImmutable();
                }
            }

            if (model.Interaction.IsSpecified)
            {
                Interaction = new MessageInteraction<SocketUser>(model.Interaction.Value.Id,
                    model.Interaction.Value.Type,
                    model.Interaction.Value.Name,
                    SocketGlobalUser.Create(Discord, state, model.Interaction.Value.User));
            }

            if (model.Flags.IsSpecified)
                Flags = model.Flags.Value;
        }

        /// <inheritdoc />
        public Task DeleteAsync(RequestOptions options = null)
            => MessageHelper.DeleteAsync(this, Discord, options);

        /// <summary>
        ///     Gets the content of the message.
        /// </summary>
        /// <returns>
        ///     Content of the message.
        /// </returns>
        public override string ToString() => Content;
        internal SocketMessage Clone() => MemberwiseClone() as SocketMessage;
#endregion

        #region IMessage
        /// <inheritdoc />
        IUser IMessage.Author => Author;
        /// <inheritdoc />
        IMessageChannel IMessage.Channel => Channel;
        /// <inheritdoc />
        IReadOnlyCollection<IAttachment> IMessage.Attachments => Attachments;
        /// <inheritdoc />
        IReadOnlyCollection<IEmbed> IMessage.Embeds => Embeds;
        /// <inheritdoc />
        IReadOnlyCollection<ulong> IMessage.MentionedChannelIds => MentionedChannels.Select(x => x.Id).ToImmutableArray();
        /// <inheritdoc />
        IReadOnlyCollection<ulong> IMessage.MentionedRoleIds => MentionedRoles.Select(x => x.Id).ToImmutableArray();
        /// <inheritdoc />
        IReadOnlyCollection<ulong> IMessage.MentionedUserIds => MentionedUsers.Select(x => x.Id).ToImmutableArray();

        /// <inheritdoc/>
        IReadOnlyCollection<IMessageComponent> IMessage.Components => Components;

        /// <inheritdoc/>
        IMessageInteraction IMessage.Interaction => Interaction;

        /// <inheritdoc />
        IReadOnlyCollection<IStickerItem> IMessage.Stickers => Stickers;


        internal void AddReaction(SocketReaction reaction)
        {
            _reactions.Add(reaction);
        }
        internal void RemoveReaction(SocketReaction reaction)
        {
            if (_reactions.Contains(reaction))
                _reactions.Remove(reaction);
        }
        internal void ClearReactions()
        {
            _reactions.Clear();
        }
        internal void RemoveReactionsForEmote(IEmote emote)
        {
            _reactions.RemoveAll(x => x.Emote.Equals(emote));
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
        public Task RemoveAllReactionsForEmoteAsync(IEmote emote, RequestOptions options = null)
            => MessageHelper.RemoveAllReactionsForEmoteAsync(this, emote, Discord, options);
        /// <inheritdoc />
        public IAsyncEnumerable<IReadOnlyCollection<IUser>> GetReactionUsersAsync(IEmote emote, int limit, RequestOptions options = null)
            => MessageHelper.GetReactionUsersAsync(this, emote, limit, Discord, options);
        #endregion
    }
}
