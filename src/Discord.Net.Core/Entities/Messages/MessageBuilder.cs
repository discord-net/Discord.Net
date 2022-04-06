using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic message builder that can build <see cref="Message"/>s.
    /// </summary>
    public class MessageBuilder
    {
        private string _content;
        private List<ISticker> _stickers;
        private List<EmbedBuilder> _embeds;

        /// <summary>
        ///     Gets or sets the content of this message
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The content is bigger than the <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        public string Content
        {
            get => _content;
            set
            {
                if (_content?.Length > DiscordConfig.MaxMessageSize)
                    throw new ArgumentOutOfRangeException(nameof(value), $"Message size must be less than or equal to {DiscordConfig.MaxMessageSize} characters");

                _content = value;
            }
        }

        /// <summary>
        ///     Gets or sets whether or not this message is TTS.
        /// </summary>
        public bool IsTTS { get; set; }

        /// <summary>
        ///     Gets or sets the embeds of this message.
        /// </summary>
        public List<EmbedBuilder> Embeds
        {
            get
            {
                if (_embeds == null)
                    _embeds = new();
                return _embeds;
            }
            set
            {
                if (value?.Count > DiscordConfig.MaxEmbedsPerMessage)
                    throw new ArgumentOutOfRangeException(nameof(value), $"Embed count must be less than or equal to {DiscordConfig.MaxEmbedsPerMessage}");

                _embeds = value;
            }
        }

        /// <summary>
        ///     Gets or sets the allowed mentions of this message.
        /// </summary>
        public AllowedMentions AllowedMentions { get; set; }

        /// <summary>
        ///     Gets or sets the message reference (reply to) of this message.
        /// </summary>
        public MessageReference MessageReference { get; set; }

        /// <summary>
        ///     Gets or sets the components of this message.
        /// </summary>
        public ComponentBuilder Components { get; set; } = new();

        /// <summary>
        ///     Gets or sets the stickers sent with this message.
        /// </summary>
        public List<ISticker> Stickers
        {
            get => _stickers;
            set
            {
                if (value?.Count > DiscordConfig.MaxStickersPerMessage)
                    throw new ArgumentOutOfRangeException(nameof(value), $"Sticker count must be less than or equal to {DiscordConfig.MaxStickersPerMessage}");

                _stickers = value;
            }
        }

        /// <summary>
        ///     Gets or sets the files sent with this message.
        /// </summary>
        public List<FileAttachment> Files { get; set; } = new();

        /// <summary>
        ///     Gets or sets the message flags.
        /// </summary>
        public MessageFlags Flags { get; set; }

        /// <summary>
        ///     Sets the <see cref="Content"/> of this message.
        /// </summary>
        /// <param name="content">The content of the message.</param>
        /// <returns>The current builder.</returns>
        public MessageBuilder WithContent(string content)
        {
            Content = content;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="IsTTS"/> of this message.
        /// </summary>
        /// <param name="isTTS">whether or not this message is tts.</param>
        /// <returns>The current builder.</returns>
        public MessageBuilder WithTTS(bool isTTS)
        {
            IsTTS = isTTS;
            return this;
        }

        public MessageBuilder WithEmbeds(params EmbedBuilder[] embeds)
        {
            Embeds = new(embeds);
            return this;
        }

        public MessageBuilder AddEmbed(EmbedBuilder embed)
        {
            if (_embeds?.Count >= DiscordConfig.MaxEmbedsPerMessage)
                throw new ArgumentOutOfRangeException(nameof(embed.Length), $"A message can only contain a maximum of {DiscordConfig.MaxEmbedsPerMessage} embeds");

            _embeds ??= new();

            _embeds.Add(embed);

            return this;
        }

        public MessageBuilder WithAllowedMentions(AllowedMentions allowedMentions)
        {
            AllowedMentions = allowedMentions;
            return this;
        }

        public MessageBuilder WithMessageReference(MessageReference reference)
        {
            MessageReference = reference;
            return this;
        }

        public MessageBuilder WithMessageReference(IMessage message)
        {
            if (message != null)
                MessageReference = new MessageReference(message.Id, message.Channel?.Id, ((IGuildChannel)message.Channel)?.GuildId);
            return this;
        }

        public MessageBuilder WithComponentBuilder(ComponentBuilder builder)
        {
            Components = builder;
            return this;
        }

        public MessageBuilder WithButton(ButtonBuilder button, int row = 0)
        {
            Components ??= new();
            Components.WithButton(button, row);
            return this;
        }

        public MessageBuilder WithButton(
            string label = null,
            string customId = null,
            ButtonStyle style = ButtonStyle.Primary,
            IEmote emote = null,
            string url = null,
            bool disabled = false,
            int row = 0)
        {
            Components ??= new();
            Components.WithButton(label, customId, style, emote, url, disabled, row);
            return this;
        }

        public MessageBuilder WithSelectMenu(SelectMenuBuilder menu, int row = 0)
        {
            Components ??= new();
            Components.WithSelectMenu(menu, row);
            return this;
        }

        public MessageBuilder WithSelectMenu(string customId, List<SelectMenuOptionBuilder> options,
            string placeholder = null, int minValues = 1, int maxValues = 1, bool disabled = false, int row = 0)
        {
            Components ??= new();
            Components.WithSelectMenu(customId, options, placeholder, minValues, maxValues, disabled, row);
            return this;
        }

        public Message Build()
        {
            var embeds = _embeds != null && _embeds.Count > 0
                ? _embeds.Select(x => x.Build()).ToImmutableArray()
                : ImmutableArray<Embed>.Empty;

            return new Message(
                _content,
                IsTTS,
                embeds,
                AllowedMentions,
                MessageReference,
                Components?.Build(),
                _stickers != null && _stickers.Any() ? _stickers.Select(x => x.Id).ToImmutableArray() : ImmutableArray<ulong>.Empty,
                Files?.ToImmutableArray() ?? ImmutableArray<FileAttachment>.Empty,
                Flags
            );
        }
    }
}
