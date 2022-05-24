using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a builder to build Discord messages with markdown with.
    /// </summary>
    public class TextBuilder
    {
        private readonly StringBuilder _builder;

        private bool _lineStart = false;

        /// <summary>
        ///     Creates a new instance of <see cref="TextBuilder"/>.
        /// </summary>
        public TextBuilder()
        {
            _builder = new();
        }

        /// <summary>
        ///     Creates a new instance of <see cref="TextBuilder"/> with a starting string appended.
        /// </summary>
        /// <param name="startingString">The string to start the builder with.</param>
        public TextBuilder(string startingString)
        {
            _builder = new(startingString);
        }

        /// <summary>
        ///     Creates a new instance of <see cref="TextBuilder"/> with a capacity and (optionally) max capacity defined.
        /// </summary>
        /// <param name="capacity">The init capacity of the underlying <see cref="StringBuilder"/>.</param>
        /// <param name="maxCapacity">The maximum capacity of the underlying <see cref="StringBuilder"/>.</param>
        public TextBuilder(int capacity, int? maxCapacity = null)
        {
            if (maxCapacity is not null)
                _builder = new(capacity, maxCapacity.Value);
            else
                _builder = new(capacity);
        }

        /// <summary>
        ///     Adds a header to the builder.
        /// </summary>
        /// <remarks>
        ///     [Note] Headers are only supported in forums, which are not released publically yet.
        /// </remarks>
        /// <param name="text">The text to be present in the header.</param>
        /// <param name="format">The header format.</param>
        /// <param name="skipLine">If the builder should skip a line when creating the next parameter.</param>
        /// <returns>The same instance with a header appended. This method will append a new line below the header.</returns>
        public TextBuilder AddHeader(string text, HeaderFormat format, bool skipLine = true)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Value cannot be null or empty.", nameof(text));

            if (skipLine)
                _builder.AppendLine();
            _builder.AppendLine(text.ToHeader(format));
            _lineStart = true;
            return this;
        }

        /// <summary>
        ///     Adds bold text to the builder.
        /// </summary>
        /// <param name="text">The text to be present in the markdown.</param>
        /// <param name="inline">If the text should be appended in the same line or if it should append to a new line.</param>
        /// <returns>The same instance with bold text appended.</returns>
        public TextBuilder AddBoldText(string text, bool inline = true)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Value cannot be null or empty.", nameof(text));
            
            Construct(Format.Bold(text), inline);
            return this;
        }

        /// <summary>
        ///     Adds bold text to the builder.
        /// </summary>
        /// <param name="builder">A builder for multiline text.</param>
        /// <param name="inline">If the text should be appended in the same line or if it should append to a new line.</param>
        /// <returns>The same instance with bold text appended.</returns>
        public TextBuilder AddBoldText(MultiLineBuilder builder, bool inline = true)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            var text = builder.Build();
            return AddBoldText(text, inline);
        }

        /// <summary>
        ///     Adds italic text to the builder.
        /// </summary>
        /// <param name="text">The text to be present in the markdown.</param>
        /// <param name="inline">If the text should be appended in the same line or if it should append to a new line.</param>
        /// <returns>The same instance with italic appended.</returns>
        public TextBuilder AddItalicText(string text, bool inline = true)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Value cannot be null or empty.", nameof(text));

            Construct(Discord.Format.Italics(text), inline);
            return this;
        }

        /// <summary>
        ///     Adds italic text to the builder.
        /// </summary>
        /// <param name="builder">A builder for multiline text.</param>
        /// <param name="inline">If the text should be appended in the same line or if it should append to a new line.</param>
        /// <returns>The same instance with italic text appended.</returns>
        public TextBuilder AddItalicText(MultiLineBuilder builder, bool inline = true)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            var text = builder.Build();
            return AddItalicText(text, inline);
        }

        /// <summary>
        ///     Adds plain text to the builder.
        /// </summary>
        /// <param name="text">The text to be present in the markdown.</param>
        /// <param name="inline">If the text should be appended in the same line or if it should append to a new line.</param>
        /// <returns>The same instance with plain text appended.</returns>
        public TextBuilder AddPlainText(string text, bool inline = true)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Value cannot be null or empty.", nameof(text));

            Construct(text, inline);
            return this;
        }

        /// <summary>
        ///     Adds plain text to the builder.
        /// </summary>
        /// <param name="builder">A builder for multiline text.</param>
        /// <param name="inline">If the text should be appended in the same line or if it should append to a new line.</param>
        /// <returns>The same instance with plain text appended.</returns>
        public TextBuilder AddPlainText(MultiLineBuilder builder, bool inline = true)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            var text = builder.Build();
            return AddPlainText(text, inline);
        }

        /// <summary>
        ///     Adds underlined text to the builder.
        /// </summary>
        /// <param name="text">The text to be present in the markdown.</param>
        /// <param name="inline">If the text should be appended in the same line or if it should append to a new line.</param>
        /// <returns>The same instance with underlined text appended.</returns>
        public TextBuilder AddUnderlinedText(string text, bool inline = true)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Value cannot be null or empty.", nameof(text));

            Construct(text.ToUnderline(), inline);
            return this;
        }

        /// <summary>
        ///     Adds underlined text to the builder.
        /// </summary>
        /// <param name="builder">A builder for multiline text.</param>
        /// <param name="inline">If the text should be appended in the same line or if it should append to a new line.</param>
        /// <returns>The same instance with underlined text appended.</returns>
        public TextBuilder AddUnderlinedText(MultiLineBuilder builder, bool inline = true)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            var text = builder.Build();
            return AddUnderlinedText(text, inline);
        }

        /// <summary>
        ///     Adds a timestamp to the builder.
        /// </summary>
        /// <param name="dateTime">The time for which this timestamp should be created.</param>
        /// <param name="style">The style of the stamp.</param>
        /// <param name="inline">If the stamp should be appended in the same line or if it should append to a new line.</param>
        /// <returns>The same instance with a timestamp appended.</returns>
        public TextBuilder AddTimestamp(DateTime dateTime, TimestampTagStyles style, bool inline = true)
        {
            Construct(TimestampTag.FromDateTime(dateTime, style).ToString(), inline);
            return this;
        }

        /// <summary>
        ///     Adds strikethrough text to the builder.
        /// </summary>
        /// <param name="text">The text to be present in the markdown.</param>
        /// <param name="inline">If the text should be appended in the same line or if it should append to a new line.</param>
        /// <returns>The same instance with striked through text appended.</returns>
        public TextBuilder AddStrikeThroughText(string text, bool inline = true)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Value cannot be null or empty.", nameof(text));

            Construct(text.ToStrikethrough(), inline);
            return this;
        }

        /// <summary>
        ///     Adds strikethrough text to the builder.
        /// </summary>
        /// <param name="builder">A builder for multiline text.</param>
        /// <param name="inline">If the text should be appended in the same line or if it should append to a new line.</param>
        /// <returns>The same instance with striked through text appended.</returns>
        public TextBuilder AddStrikeThroughText(MultiLineBuilder builder, bool inline = true)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            var text = builder.Build();
            return AddStrikeThroughText(text, inline);
        }

        /// <summary>
        ///     Adds a spoiler to the builder.
        /// </summary>
        /// <param name="text">The text to be present in the markdown.</param>
        /// <param name="inline">If the text should be appended in the same line or if it should append to a new line.</param>
        /// <returns>The same instance with a spoiler appended.</returns>
        public TextBuilder AddSpoiler(string text, bool inline = true)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Value cannot be null or empty.", nameof(text));

            Construct(text.ToSpoiler(), inline);
            return this;
        }

        /// <summary>
        ///     Adds a spoiler to the builder.
        /// </summary>
        /// <param name="builder">A builder for multiline text.</param>
        /// <param name="inline">If the text should be appended in the same line or if it should append to a new line.</param>
        /// <returns>The same instance with a spoiler appended.</returns>
        public TextBuilder AddSpoiler(MultiLineBuilder builder, bool inline = true)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            var text = builder.Build();
            return AddSpoiler(text, inline);
        }

        /// <summary>
        ///     Adds a quote to the builder.
        /// </summary>
        /// <param name="text">The text to be present in the markdown.</param>
        /// <param name="skipLine">If the builder should skip a line when creating the next parameter.</param>
        /// <returns>The same instance with a quote appended. This method will append a new line below the quote.</returns>
        public TextBuilder AddQuote(string text, bool skipLine = true)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Value cannot be null or empty.", nameof(text));

            if (skipLine)
                _builder.AppendLine();
            _builder.AppendLine(text.ToQuote());
            _lineStart = true;
            return this;
        }

        /// <summary>
        ///     Adds a quote to the builder.
        /// </summary>
        /// <param name="builder">A builder for multiline text.</param>
        /// <param name="inline">If the text should be appended in the same line or if it should append to a new line.</param>
        /// <returns>The same instance with a quote appended.</returns>
        public TextBuilder AddQuote(MultiLineBuilder builder, bool inline = true)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            var text = builder.Build();
            return AddQuote(text, inline);
        }

        /// <summary>
        ///     Adds a block quote to the builder.
        /// </summary>
        /// <param name="text">The text to be present in the markdown.</param>
        /// <param name="skipLine">If the builder should skip a line when creating the next parameter.</param>
        /// <returns>The same instance with a block quote appended. This method will append a new line below the quote.</returns>
        public TextBuilder AddBlockQuote(string text, bool skipLine = true)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Value cannot be null or empty.", nameof(text));

            if (skipLine)
                _builder.AppendLine();
            _builder.AppendLine(text.ToBlockQuote());
            _lineStart = true;
            return this;
        }

        /// <summary>
        ///     Adds a block quote to the builder.
        /// </summary>
        /// <param name="builder">A builder for multiline text.</param>
        /// <param name="skipLine">If the builder should skip a line when creating the next parameter.</param>
        /// <returns>The same instance with a block quote appended. This method will append a new line below the quote.</returns>
        public TextBuilder AddBlockQuote(MultiLineBuilder builder, bool skipLine = true)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            var text = builder.Build();
            return AddBlockQuote(text, skipLine);
        }

        /// <summary>
        ///     Adds code marked text to the builder.
        /// </summary>
        /// <param name="text">The text to be present in the markdown.</param>
        /// <param name="inline">If the text should be appended in the same line or if it should append to a new line.</param>
        /// <returns>The same instance with code marked text appended.</returns>
        public TextBuilder AddCode(string text, bool inline = false)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Value cannot be null or empty.", nameof(text));

            Construct(text.ToCode(), inline);
            return this;
        }

        /// <summary>
        ///     Adds code marked text to the builder.
        /// </summary>
        /// <param name="builder">A builder for multiline text.</param>
        /// <param name="inline">If the text should be appended in the same line or if it should append to a new line.</param>
        /// <returns>The same instance with code marked text appended.</returns>
        public TextBuilder AddCode(MultiLineBuilder builder, bool inline = true)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            var text = builder.Build();
            return AddCode(text, inline);
        }

        /// <summary>
        ///     Adds a code block to the builder.
        /// </summary>
        /// <param name="text">The text to be present in the markdown.</param>
        /// <param name="lang">The language in which this code should be presented.</param>
        /// <param name="skipLine">If the builder should skip a line when creating the next parameter.</param>
        /// <returns>The same instance with a code block appended. This method will append a new line below the block.</returns>
        public TextBuilder AddCodeBlock(string text, CodeLanguage? lang = null, bool skipLine = true)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Value cannot be null or empty.", nameof(text));

            lang ??= CodeLanguage.None;
            if (skipLine)
                _builder.AppendLine();
            _builder.AppendLine(text.ToCodeBlock(lang));
            _lineStart = true;
            return this;
        }

        /// <summary>
        ///     Adds a code block to the builder.
        /// </summary>
        /// <param name="builder">A builder for multiline text.</param>
        /// <param name="lang">The language in which this code should be presented.</param>
        /// <param name="skipLine">If the builder should skip a line when creating the next parameter.</param>
        /// <returns>The same instance with a code block appended. This method will append a new line below the quote.</returns>
        public TextBuilder AddCodeBlock(MultiLineBuilder builder, CodeLanguage? lang = null, bool skipLine = true)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            var text = builder.Build();
            return AddCodeBlock(text, lang, skipLine);
        }

        /// <summary>
        ///     Adds an emote to the builder.
        /// </summary>
        /// <param name="emote">The emote to add.</param>
        /// <param name="inline">If the emote should be appended in the same line or if it should append to a new line.</param>
        /// <returns>The same instance with an emote appended.</returns>
        public TextBuilder AddEmote(IEmote emote, bool inline = false)
        {
            if (emote is null)
                throw new ArgumentNullException(nameof(emote));
            var str = emote switch
            {
                Emote ee => ee.ToString(),
                Emoji ei => ei.ToString(),
                _ => null
            };

            Construct(str, inline);
            return this;
        }

        /// <summary>
        ///     Adds a range of emotes to the builder.
        /// </summary>
        /// <param name="seperator">The seperator to join the emotes with.</param>
        /// <param name="inline">If the emotes should be appended in the same line or if it should append to a new line.</param>
        /// <param name="emotes">The range of emotes to add.</param>
        /// <returns>The same instance with a range of emotes appended.</returns>
        public TextBuilder AddEmotes(string seperator, bool inline = false, params IEmote[] emotes)
        {
            if (!emotes.Any())
                throw new ArgumentException("No values were found in the passed selection", nameof(emotes));

            var str = string.Join(seperator, emotes.Select(x =>
            {
                return x switch
                {
                    Emote emote => emote.ToString(),
                    Emoji emoji => emoji.ToString(),
                    _ => throw new ArgumentNullException(nameof(emotes)),
                };
            }));
            Construct(str, inline);
            return this;
        }

        /// <summary>
        ///     Starts the next query to the builder on a new line.
        /// </summary>
        /// <returns>The same instance with an empty line appended.</returns>
        public TextBuilder AddNewline()
        {
            _builder.AppendLine();
            _lineStart = true;
            return this;
        }

        /// <summary>
        ///     Builds a Discord message string from this instance.
        /// </summary>
        /// <returns>The string to send to Discord.</returns>
        public string Build()
            => _builder.ToString();

        private void Construct(string text, bool inline)
        {
            if (_builder.Length + text.Length > DiscordConfig.MaxMessageSize)
                throw new ArgumentOutOfRangeException(nameof(text), $"Maximum message length of {DiscordConfig.MaxMessageSize} has been reached.");

            if (inline)
            {
                if (!_lineStart)
                    text = " " + text;

                else
                    _lineStart = false;

                _builder.Append(text); // add a space to define 
            }
            else
            {
                if (_lineStart)
                    _lineStart = false;
                _builder.AppendLine();
                _builder.Append(text);
            }
        }

        /// <summary>
        ///     Builds the underlying <see cref="StringBuilder"/> to a string.
        /// </summary>
        /// <remarks>
        ///     This method has the same functionality as <see cref="Build"/>.
        /// </remarks>
        /// <returns>The string to send to Discord.</returns>
        public override string ToString()
            => _builder.ToString();
    }
}
