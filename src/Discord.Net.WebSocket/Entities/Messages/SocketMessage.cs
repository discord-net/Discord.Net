using Discord.Rest;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.IMessageModel;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based message.
    /// </summary>
    public abstract class SocketMessage : SocketEntity<ulong>, IMessage, ICached<Model>
    {
        #region SocketMessage
        internal bool IsFreed { get; set; }
        private long _timestampTicks;
        private readonly List<SocketReaction> _reactions = new List<SocketReaction>();
        private ulong[] _userMentionIds;
        private ulong _channelId;
        private ulong _guildId;
        private ulong _authorId;
        private bool _isWebhook;
        //private ImmutableArray<SocketUser> _userMentions = ImmutableArray.Create<SocketUser>();

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
        public virtual ulong? ApplicationId { get; private set; }

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
        /// <remarks>
        ///     The returned enumerable will preform cache lookups per enumeration.
        /// </remarks>
        /// <returns>
        ///     Collection of WebSocket-based users.
        /// </returns>
        public IEnumerable<SocketUser> MentionedUsers => Discord.StateManager.UserStore.GetEnumerable(_userMentionIds); // TODO: async counterpart?
        /// <inheritdoc />
        public DateTimeOffset Timestamp => DateTimeUtils.FromTicks(_timestampTicks);

        internal SocketMessage(DiscordSocketClient discord, ulong id, ISocketMessageChannel channel, SocketUser author, MessageSource source)
            : base(discord, id)
        {
            Channel = channel;
            Author = author;
            Source = source;
        }

        //internal static SocketMessage Create(DiscordSocketClient discord, Model model, ulong channelId)
        //{

        //}

        internal static SocketMessage Create(DiscordSocketClient discord, ClientStateManager state, SocketUser author, ISocketMessageChannel channel, Model model)
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
        internal virtual void Update(Model model)
        {
            Type = model.Type;

            _timestampTicks = model.Timestamp;
            ApplicationId = model.ApplicationId;
            Content = model.Content;
            _userMentionIds = model.UserMentionIds;

            if (model.Application != null)
            {
                // create a new Application from the API model
                Application = new MessageApplication()
                {
                    Id = model.Application.Id,
                    CoverImage = model.Application.CoverImage,
                    Description = model.Application.Description,
                    Icon = model.Application.Icon,
                    Name = model.Application.Name
                };
            }

            if (model.Activity != null)
            {
                // create a new Activity from the API model
                Activity = new MessageActivity()
                {
                    Type = model.Activity.Type.Value,
                    PartyId = model.Activity.PartyId
                };
            }

            if (model.ReferenceMessageId.HasValue)
            {
                // Creates a new Reference from the API model
                Reference = new MessageReference
                {
                    GuildId = model.ReferenceMessageGuildId.ToOptional(),
                    InternalChannelId = model.ReferenceMessageChannelId.ToOptional(),
                    MessageId = model.ReferenceMessageId.ToOptional()
                };
            }

            if (model.Components != null && model.Components.Length > 0)
            {
                Components = model.Components.Select(x => new ActionRowComponent(x.Components.Select<IMessageComponentModel, IMessageComponent>(y =>
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

            if (model.InteractionId.HasValue)
            {
                Interaction = new MessageInteraction<SocketUser>(model.InteractionId.Value,
                    model.InteractionType.Value,
                    model.InteractionName,
                    model.InteractionUserId.Value,
                    Discord.StateManager.UserStore.Get);
            }

            Flags = model.Flags;
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

        #region Cache

        internal class CacheModel : Model
        {
            public MessageType Type { get; set; }
            public ulong ChannelId { get; set; }
            public ulong? GuildId { get; set; }
            public ulong AuthorId { get; set; }
            public bool IsWebhookMessage { get; set; }
            public string Content { get; set; }
            public long Timestamp { get; set; }
            public long? EditedTimestamp { get; set; }
            public bool IsTextToSpeech { get; set; }
            public bool MentionEveryone { get; set; }
            public ulong[] UserMentionIds { get; set; }
            public AttachmentModel[] Attachments { get; set; }
            public EmbedModel[] Embeds { get; set; }
            public ReactionModel[] Reactions { get; set; } // TODO: seperate store?
            public bool Pinned { get; set; }
            public MessageActivityModel Activity { get; set; }
            public PartialApplicationModel Application { get; set; }
            public ulong? ApplicationId { get; set; }
            public ulong? ReferenceMessageId { get; set; }
            public ulong? ReferenceMessageChannelId { get; set; }
            public ulong? ReferenceMessageGuildId { get; set; }
            public MessageFlags Flags { get; set; }
            public ulong? InteractionId { get; set; }
            public string InteractionName { get; set; }
            public InteractionType? InteractionType { get; set; }
            public ulong? InteractionUserId { get; set; }
            public MessageComponentModel[] Components { get; set; }
            public StickerItemModel[] Stickers { get; set; }
            public ulong Id { get; set; }

            internal class AttachmentModel : IAttachmentModel
            {
                public string FileName { get; set; }
                public string Description { get; set; }
                public string ContentType { get; set; }
                public int Size { get; set; }
                public string Url { get; set; }
                public string ProxyUrl { get; set; }
                public int? Height { get; set; }
                public int? Width { get; set; }
                public bool Ephemeral { get; set; }
                public ulong Id { get; set; }
            }
            internal class EmbedModel : IEmbedModel
            {
                public string Title { get; set; }
                public EmbedType Type { get; set; }
                public string Description { get; set; }
                public string Url { get; set; }
                public long? Timestamp { get; set; }
                public uint? Color { get; set; }
                public string FooterText { get; set; }
                public string FooterIconUrl { get; set; }
                public string FooterProxyUrl { get; set; }
                public string ProviderName { get; set; }
                public string ProviderUrl { get; set; }
                public string AuthorName { get; set; }
                public string AuthorUrl { get; set; }
                public string AuthorIconUrl { get; set; }
                public string AuthorProxyIconUrl { get; set; }
                public EmbedMediaModel Image { get; set; }
                public EmbedMediaModel Thumbnail { get; set; }
                public EmbedMediaModel Video { get; set; }
                public EmbedFieldModel[] Fields { get; set; }

                IEmbedMediaModel IEmbedModel.Image { get => Image; set => Image = value.InterfaceCopy<EmbedMediaModel>(); }
                IEmbedMediaModel IEmbedModel.Thumbnail { get => Thumbnail; set => Thumbnail = value.InterfaceCopy<EmbedMediaModel>(); }
                IEmbedMediaModel IEmbedModel.Video { get => Video; set => Video = value.InterfaceCopy<EmbedMediaModel>(); }
                IEmbedFieldModel[] IEmbedModel.Fields { get => Fields; set => value?.Select(x => x.InterfaceCopy<EmbedFieldModel>()); }

                internal class EmbedMediaModel : IEmbedMediaModel
                {
                    public string Url { get; set; }
                    public string ProxyUrl { get; set; }
                    public int? Height { get; set; }
                    public int? Width { get; set; }
                }
                internal class EmbedFieldModel : IEmbedFieldModel
                {
                    public string Name { get; set; }
                    public string Value { get; set; }
                    public bool Inline { get; set; }
                }
            }
            internal class ReactionModel : IReactionMetadataModel
            {
                public IEmojiModel Emoji { get; set; }
                public ulong[] Users { get; set; }
            }
            internal class MessageActivityModel : IMessageActivityModel
            {
                public MessageActivityType? Type { get; set; }
                public string PartyId { get; set; }
            }
            internal class PartialApplicationModel : IPartialApplicationModel
            {
                public string Name { get; set; }
                public string Icon { get; set; }
                public string Description { get; set; }
                public string CoverImage { get; set; }
                public ulong Id { get; set; }
            }
            internal class MessageComponentModel : IMessageComponentModel
            {
                public ComponentType Type { get; set; }
                public string CustomId { get; set; }
                public bool? Disabled { get; set; }
                public ButtonStyle? Style { get; set; }
                public string Label { get; set; }
                public ulong? EmojiId { get; set; }
                public string EmojiName { get; set; }
                public bool? EmojiAnimated { get; set; }
                public string Url { get; set; }
                public MessageComponentOptionModel[] Options { get; set; }
                public string Placeholder { get; set; }
                public int? MinValues { get; set; }
                public int? MaxValues { get; set; }
                public MessageComponentModel[] Components { get; set; }
                public int? MinLength { get; set; }
                public int? MaxLength { get; set; }
                public bool? Required { get; set; }
                public string Value { get; set; }
                
                internal class MessageComponentOptionModel : IMessageComponentOptionModel
                {
                    public string Label { get; set; }
                    public string Value { get; set; }
                    public string Description { get; set; }
                    public ulong? EmojiId { get; set; }
                    public string EmojiName { get; set; }
                    public bool? EmojiAnimated { get; set; }
                    public bool? Default { get; set; }
                }

                IMessageComponentOptionModel[] IMessageComponentModel.Options { get => Options; set => Options = value.Select(x => x.InterfaceCopy(new MessageComponentOptionModel())).ToArray(); }
                IMessageComponentModel[] IMessageComponentModel.Components { get => Components; set => Components = value.Select(x => x.InterfaceCopy(new MessageComponentModel())).ToArray(); }
            }
            internal class StickerItemModel : IStickerItemModel
            {
                public ulong Id { get; set; }
                public string Name { get; set; }
                public StickerFormatType Format { get; set; }
            }

            IAttachmentModel[] Model.Attachments { get => Attachments; set => Attachments = value.Select(x => x.InterfaceCopy<AttachmentModel>()).ToArray(); }
            IEmbedModel[] Model.Embeds { get => Embeds; set => Embeds = value.Select(x => x.InterfaceCopy<EmbedModel>()).ToArray(); }
            IReactionMetadataModel[] Model.Reactions { get => Reactions; set => Reactions = value.Select(x => x.InterfaceCopy<ReactionModel>()).ToArray(); }
            IMessageActivityModel Model.Activity { get => Activity; set => Activity = value.InterfaceCopy<MessageActivityModel>(); }
            IPartialApplicationModel Model.Application { get => Application; set => Application = value.InterfaceCopy<PartialApplicationModel>(); }
            IMessageComponentModel[] Model.Components { get => Components; set => Components = value.Select(x => x.InterfaceCopy<MessageComponentModel>()).ToArray(); }
            IStickerItemModel[] Model.Stickers { get => Stickers; set => Stickers = value.Select(x => x.InterfaceCopy<StickerItemModel>()).ToArray(); }
        }

        internal virtual Model ToModel()
        {
            var model = Discord.StateManager.GetModel<Model>();
            model.Content = Content;
            model.Type = Type;
            model.ChannelId = _channelId;
            model.GuildId = _guildId;
            model.AuthorId = _authorId;
            model.IsWebhookMessage = _isWebhook;
            model.Timestamp = _timestampTicks;
            model.IsTextToSpeech = IsTTS;
            model.MentionEveryone = MentionedEveryone;
            model.UserMentionIds = _userMentionIds;
            model.ApplicationId = ApplicationId;
            model.Flags = Flags ?? MessageFlags.None;
            model.Id = Id;

            if(Interaction != null)
            {
                model.InteractionName = Interaction.Name;
                model.InteractionId = Interaction.Id;
                model.InteractionType = Interaction.Type;
                model.InteractionUserId = Interaction.UserId;
            }

            if(Reference != null)
            {
                model.ReferenceMessageId = Reference.MessageId.ToNullable();
                model.ReferenceMessageGuildId = Reference.GuildId.ToNullable();
                model.ReferenceMessageChannelId = Reference.ChannelId;
            }

            model.Attachments = Attachments.Select(x =>
            {
                var attachmentModel = Discord.StateManager.GetModel<IAttachmentModel>();
                attachmentModel.Width = x.Width;
                attachmentModel.Height = x.Height;
                attachmentModel.Size = x.Size;
                attachmentModel.Description = x.Description;
                attachmentModel.Ephemeral = x.Ephemeral;
                attachmentModel.FileName = x.Filename;
                attachmentModel.Url = x.Url;
                attachmentModel.ContentType = x.ContentType;
                attachmentModel.Id = x.Id;
                attachmentModel.ProxyUrl = x.ProxyUrl;

                return attachmentModel;
            }).ToArray();

            model.Embeds = Embeds.Select(x =>
            {
                var embedModel = Discord.StateManager.GetModel<IEmbedModel>();

                embedModel.AuthorName = x.Author?.Name;
                embedModel.AuthorProxyIconUrl = x.Author?.ProxyIconUrl;
                embedModel.AuthorIconUrl = x.Author?.IconUrl;
                embedModel.AuthorUrl = x.Author?.Url;

                embedModel.Color = x.Color;
                embedModel.Description = x.Description;
                embedModel.Title = x.Title;
                embedModel.Timestamp = x.Timestamp?.UtcTicks;
                embedModel.Type = x.Type;
                embedModel.Url = x.Url;

                embedModel.ProviderName = x.Provider?.Name;
                embedModel.ProviderUrl = x.Provider?.Url;

                embedModel.FooterIconUrl = x.Footer?.IconUrl;
                embedModel.FooterProxyUrl = x.Footer?.ProxyUrl;
                embedModel.FooterText = x.Footer?.Text;

                var image = Discord.StateManager.GetModel<IEmbedMediaModel>();
                image.Width = x.Image?.Width;
                image.Height = x.Image?.Height;
                image.Url = x.Image?.Url;
                image.ProxyUrl = x.Image?.ProxyUrl;

                embedModel.Image = image;

                var thumbnail = Discord.StateManager.GetModel<IEmbedMediaModel>();
                thumbnail.Width = x.Thumbnail?.Width;
                thumbnail.Height = x.Thumbnail?.Height;
                thumbnail.Url = x.Thumbnail?.Url;
                thumbnail.ProxyUrl = x.Thumbnail?.ProxyUrl;

                embedModel.Thumbnail = thumbnail;

                var video = Discord.StateManager.GetModel<IEmbedMediaModel>();
                video.Width = x.Video?.Width;
                video.Height = x.Video?.Height;
                video.Url = x.Video?.Url;

                embedModel.Video = video;

                embedModel.Fields = x.Fields.Select(x =>
                {
                    var fieldModel = Discord.StateManager.GetModel<IEmbedFieldModel>();
                    fieldModel.Name = x.Name;
                    fieldModel.Value = x.Value;
                    fieldModel.Inline = x.Inline;
                    return fieldModel;
                }).ToArray();

                return embedModel;
            }).ToArray();

            model.Reactions = _reactions.GroupBy(x => x.Emote).Select(x =>
            {
                var reactionMetadataModel = Discord.StateManager.GetModel<IReactionMetadataModel>();
                reactionMetadataModel.Emoji = x.Key.ToModel(Discord.StateManager.GetModel<IEmojiModel>());
                reactionMetadataModel.Users = x.Select(x => x.UserId).ToArray();
                return reactionMetadataModel;
            }).ToArray();

            var activityModel = Discord.StateManager.GetModel<IMessageActivityModel>();
            activityModel.PartyId = Activity?.PartyId;
            activityModel.Type = Activity?.Type;
            model.Activity = activityModel;

            var applicationModel = Discord.StateManager.GetModel<IPartialApplicationModel>();
            applicationModel.Description = Application.Description;
            applicationModel.Name = Application.Name;
            applicationModel.CoverImage = Application.CoverImage;
            applicationModel.Id = Application.Id;
            applicationModel.Icon = Application.Icon;
            model.Application = applicationModel;

            return model;
        }

        ~SocketMessage() => Dispose();
        public void Dispose()
        {
            if (IsFreed)
                return;

            IsFreed = true;

            GC.SuppressFinalize(this);

            if (Discord.StateManager.TryGetMessageStore(Channel.Id, out var store))
                store.RemoveReference(Id);
        }


        void ICached<Model>.Update(Model model) => Update(model);
        Model ICached<Model>.ToModel() => ToModel();

        bool ICached.IsFreed => IsFreed;
        #endregion
    }
}
