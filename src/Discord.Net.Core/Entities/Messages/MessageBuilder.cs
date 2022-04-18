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
        private List<ISticker> _stickers = new();
        private List<EmbedBuilder> _embeds = new();
        private List<FileAttachment> _files = new();
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
            get => _embeds;
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
        public List<FileAttachment> Files
        {
            get => _files;
            set
            {
                if(value?.Count > DiscordConfig.MaxFilesPerMessage)
                    throw new ArgumentOutOfRangeException(nameof(value), $"File count must be less than or equal to {DiscordConfig.MaxFilesPerMessage}");
            }
        }

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

        /// <summary>
        ///     Sets the <see cref="Embeds"/> of this message.
        /// </summary>
        /// <param name="embeds">The embeds to be put in this message.</param>
        /// <returns>The current builder.</returns>
        /// <exception cref="ArgumentOutOfRangeException">A message can only contain a maximum of <see cref="DiscordConfig.MaxEmbedsPerMessage"/> embeds.</exception>
        public MessageBuilder WithEmbeds(params EmbedBuilder[] embeds)
        {
            Embeds = new(embeds);
            return this;
        }

        /// <summary>
        ///     Adds an embed builder to the current <see cref="Embeds"/>.
        /// </summary>
        /// <param name="embed">The embed builder to add</param>
        /// <returns>The current builder.</returns>
        /// <exception cref="ArgumentOutOfRangeException">A message can only contain a maximum of <see cref="DiscordConfig.MaxEmbedsPerMessage"/> embeds.</exception>
        public MessageBuilder AddEmbed(EmbedBuilder embed)
        {
            if (_embeds?.Count >= DiscordConfig.MaxEmbedsPerMessage)
                throw new ArgumentOutOfRangeException(nameof(embed.Length), $"A message can only contain a maximum of {DiscordConfig.MaxEmbedsPerMessage} embeds");

            _embeds ??= new();

            _embeds.Add(embed);

            return this;
        }

        /// <summary>
        ///     Sets the <see cref="AllowedMentions"/> for this message.
        /// </summary>
        /// <param name="allowedMentions">The allowed mentions for this message.</param>
        /// <returns>The current builder.</returns>
        public MessageBuilder WithAllowedMentions(AllowedMentions allowedMentions)
        {
            AllowedMentions = allowedMentions;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="MessageReference"/> for this message.
        /// </summary>
        /// <param name="reference">The message reference (reply-to) for this message.</param>
        /// <returns>The current builder.</returns>
        public MessageBuilder WithMessageReference(MessageReference reference)
        {
            MessageReference = reference;
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="MessageReference"/> for this current message.
        /// </summary>
        /// <param name="message">The message to set as a reference.</param>
        /// <returns>The current builder.</returns>
        public MessageBuilder WithMessageReference(IMessage message)
        {
            if (message != null)
                MessageReference = new MessageReference(message.Id, message.Channel?.Id, ((IGuildChannel)message.Channel)?.GuildId);
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="Components"/> for this message.
        /// </summary>
        /// <param name="builder">The component builder to set.</param>
        /// <returns>The current builder.</returns>
        public MessageBuilder WithComponentBuilder(ComponentBuilder builder)
        {
            Components = builder;
            return this;
        }

        /// <summary>
        ///     Adds a button to the current <see cref="Components"/>.
        /// </summary>
        /// <param name="button">The button builder to add.</param>
        /// <param name="row">The optional row to place the button on.</param>
        /// <returns>The current builder.</returns>
        public MessageBuilder WithButton(ButtonBuilder button, int row = 0)
        {
            Components ??= new();
            Components.WithButton(button, row);
            return this;
        }

        /// <summary>
        ///     Adds a button to the current <see cref="Components"/>.
        /// </summary>
        /// <param name="label">The label text for the newly added button.</param>
        /// <param name="style">The style of this newly added button.</param>
        /// <param name="emote">A <see cref="IEmote"/> to be used with this button.</param>
        /// <param name="customId">The custom id of the newly added button.</param>
        /// <param name="url">A URL to be used only if the <see cref="ButtonStyle"/> is a Link.</param>
        /// <param name="disabled">Whether or not the newly created button is disabled.</param>
        /// <param name="row">The row the button should be placed on.</param>
        /// <returns>The current builder.</returns>
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

        /// <summary>
        ///     Adds a select menu to the current <see cref="Components"/>.
        /// </summary>
        /// <param name="menu">The select menu builder to add.</param>
        /// <param name="row">The optional row to place the select menu on.</param>
        /// <returns>The current builder.</returns>
        public MessageBuilder WithSelectMenu(SelectMenuBuilder menu, int row = 0)
        {
            Components ??= new();
            Components.WithSelectMenu(menu, row);
            return this;
        }

        /// <summary>
        ///     Adds a select menu to the current <see cref="Components"/>.
        /// </summary>
        /// <param name="customId">The custom id of the menu.</param>
        /// <param name="options">The options of the menu.</param>
        /// <param name="placeholder">The placeholder of the menu.</param>
        /// <param name="minValues">The min values of the placeholder.</param>
        /// <param name="maxValues">The max values of the placeholder.</param>
        /// <param name="disabled">Whether or not the menu is disabled.</param>
        /// <param name="row">The row to add the menu to.</param>
        /// <returns>The current builder.</returns>
        public MessageBuilder WithSelectMenu(string customId, List<SelectMenuOptionBuilder> options,
            string placeholder = null, int minValues = 1, int maxValues = 1, bool disabled = false, int row = 0)
        {
            Components ??= new();
            Components.WithSelectMenu(customId, options, placeholder, minValues, maxValues, disabled, row);
            return this;
        }

        /// <summary>
        ///     Sets the <see cref="Files"/> of this message.
        /// </summary>
        /// <param name="files">The file collection to set.</param>
        /// <returns>The current builder.</returns>
        public MessageBuilder WithFiles(IEnumerable<FileAttachment> files)
        {
            Files = new List<FileAttachment>(files);
            return this;
        }

        /// <summary>
        ///     Adds a file to the current collection <see cref="Files"/>.
        /// </summary>
        /// <param name="file">The file to add.</param>
        /// <returns>The current builder.</returns>
        public MessageBuilder AddFile(FileAttachment file)
        {
            Files.Add(file);
            return this;
        }

        /// <summary>
        ///     Builds this message builder to a <see cref="Message"/> that can be sent across the discord api.
        /// </summary>
        /// <returns>A <see cref="Message"/> that can be sent to the discord api.</returns>
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
                _stickers,
                Files?.ToImmutableArray() ?? ImmutableArray<FileAttachment>.Empty,
                Flags
            );
        }
    }
}
