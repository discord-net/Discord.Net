using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Message;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based message.
    /// </summary>
    public abstract class RestMessage : RestEntity<ulong>, IMessage, IUpdateable
    {
        private long _timestampTicks;
        private ImmutableArray<RestReaction> _reactions = ImmutableArray.Create<RestReaction>();
        private ImmutableArray<RestUser> _userMentions = ImmutableArray.Create<RestUser>();

        /// <inheritdoc />
        public IMessageChannel Channel { get; }
        /// <summary>
        ///     Gets the Author of the message.
        /// </summary>
        public IUser Author { get; }
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

        /// <summary>
        ///     Gets a collection of the <see cref="Attachment"/>'s on the message.
        /// </summary>
        public virtual IReadOnlyCollection<Attachment> Attachments => ImmutableArray.Create<Attachment>();
        /// <summary>
        ///     Gets a collection of the <see cref="Embed"/>'s on the message.
        /// </summary>
        public virtual IReadOnlyCollection<Embed> Embeds => ImmutableArray.Create<Embed>();
        /// <inheritdoc />
        public virtual IReadOnlyCollection<ulong> MentionedChannelIds => ImmutableArray.Create<ulong>();
        /// <inheritdoc />
        public virtual IReadOnlyCollection<ulong> MentionedRoleIds => ImmutableArray.Create<ulong>();
        /// <inheritdoc />
        public virtual IReadOnlyCollection<ITag> Tags => ImmutableArray.Create<ITag>();
        /// <inheritdoc />
        public virtual IReadOnlyCollection<StickerItem> Stickers => ImmutableArray.Create<StickerItem>();

        /// <inheritdoc />
        public DateTimeOffset Timestamp => DateTimeUtils.FromTicks(_timestampTicks);
        /// <inheritdoc />
        public MessageActivity Activity { get; private set; }
        /// <inheritdoc />
        public MessageApplication Application { get; private set; }
        /// <inheritdoc />
        public MessageReference Reference { get; private set; }

        /// <summary>
        ///     Gets the interaction this message is a response to.
        /// </summary>
        public MessageInteraction<RestUser> Interaction { get; private set; }
        /// <inheritdoc />
        public MessageFlags? Flags { get; private set; }
        /// <inheritdoc/>
        public MessageType Type { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyCollection<ActionRowComponent> Components { get; private set; }
        /// <summary>
        ///     Gets a collection of the mentioned users in the message.
        /// </summary>
        public IReadOnlyCollection<RestUser> MentionedUsers => _userMentions;

        internal RestMessage(BaseDiscordClient discord, ulong id, IMessageChannel channel, IUser author, MessageSource source)
            : base(discord, id)
        {
            Channel = channel;
            Author = author;
            Source = source;
        }
        internal static RestMessage Create(BaseDiscordClient discord, IMessageChannel channel, IUser author, Model model)
        {
            if (model.Type == MessageType.Default ||
                model.Type == MessageType.Reply ||
                model.Type == MessageType.ApplicationCommand ||
                model.Type == MessageType.ThreadStarterMessage)
                return RestUserMessage.Create(discord, channel, author, model);
            else
                return RestSystemMessage.Create(discord, channel, author, model);
        }
        internal virtual void Update(Model model)
        {
            Type = model.Type;

            if (model.Timestamp.IsSpecified)
                _timestampTicks = model.Timestamp.Value.UtcTicks;

            if (model.Content.IsSpecified)
                Content = model.Content.Value;

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

            if (model.Flags.IsSpecified)
                Flags = model.Flags.Value;

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

            if (model.Interaction.IsSpecified)
            {
                Interaction = new MessageInteraction<RestUser>(model.Interaction.Value.Id,
                    model.Interaction.Value.Type,
                    model.Interaction.Value.Name,
                    RestUser.Create(Discord, model.Interaction.Value.User));
            }

            if (model.UserMentions.IsSpecified)
            {
                var value = model.UserMentions.Value;
                if (value.Length > 0)
                {
                    var newMentions = ImmutableArray.CreateBuilder<RestUser>(value.Length);
                    for (int i = 0; i < value.Length; i++)
                    {
                        var val = value[i];
                        if (val != null)
                            newMentions.Add(RestUser.Create(Discord, val));
                    }
                    _userMentions = newMentions.ToImmutable();
                }
            }
        }
        /// <inheritdoc />
        public async Task UpdateAsync(RequestOptions options = null)
        {
            var model = await Discord.ApiClient.GetChannelMessageAsync(Channel.Id, Id, options).ConfigureAwait(false);
            Update(model);
        }
        /// <inheritdoc />
        public Task DeleteAsync(RequestOptions options = null)
            => MessageHelper.DeleteAsync(this, Discord, options);

        /// <summary>
        ///     Gets the <see cref="Content"/> of the message.
        /// </summary>
        /// <returns>
        ///     A string that is the <see cref="Content"/> of the message.
        /// </returns>
        public override string ToString() => Content;

        IUser IMessage.Author => Author;
        /// <inheritdoc />
        IReadOnlyCollection<IAttachment> IMessage.Attachments => Attachments;
        /// <inheritdoc />
        IReadOnlyCollection<IEmbed> IMessage.Embeds => Embeds;
        /// <inheritdoc />
        IReadOnlyCollection<ulong> IMessage.MentionedUserIds => MentionedUsers.Select(x => x.Id).ToImmutableArray();

        /// <inheritdoc/>
        IReadOnlyCollection<IMessageComponent> IMessage.Components => Components;

        /// <inheritdoc/>
        IMessageInteraction IMessage.Interaction => Interaction;

        /// <inheritdoc />
        IReadOnlyCollection<IStickerItem> IMessage.Stickers => Stickers;

        /// <inheritdoc />
        public IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions => _reactions.ToDictionary(x => x.Emote, x => new ReactionMetadata { ReactionCount = x.Count, IsMe = x.Me });

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
    }
}
